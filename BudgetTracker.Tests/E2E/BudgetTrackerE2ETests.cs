using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BudgetTracker.Tests.E2E;

// ---------------------------------------------------------------------------
// Playwright end-to-end tests for BudgetTracker
//
// BEFORE RUNNING THESE TESTS you need to do two things:
//
// 1. Install the Playwright browser binaries (one-time setup).
//    Build the test project first, then run this command from the solution root:
//
//      pwsh BudgetTracker.Tests\bin\Debug\net8.0\playwright.ps1 install
//
// 2. Set two environment variables for the Auth0 test user:
//
//      $env:TEST_AUTH0_EMAIL    = "your-test-user@example.com"
//      $env:TEST_AUTH0_PASSWORD = "your-test-user-password"
//
//    Create a dedicated test account in your Auth0 tenant — never use a real
//    user's credentials in automated tests.
//
// 3. Make sure the app is already running on https://localhost:7266.
// ---------------------------------------------------------------------------

// IClassFixture<PlaywrightFixture> means xUnit shares one browser instance
// across all tests in this class instead of starting a new one for each test.
public class BudgetTrackerE2ETests : IClassFixture<PlaywrightFixture>, IAsyncLifetime
{
    // The base URL of the running app — matches the HTTPS port in launchSettings.json
    private const string BaseUrl = "https://localhost:7266";

    private readonly PlaywrightFixture _fixture;

    // Each test gets its own browser context (think of it as a fresh browser window
    // with no cookies or saved login state from other tests)
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public BudgetTrackerE2ETests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    // Runs before each individual test: open a new browser context and log in
    public async Task InitializeAsync()
    {
        // IgnoreHTTPSErrors = true lets Playwright trust the self-signed
        // development certificate that .NET generates for localhost
        _context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });

        _page = await _context.NewPageAsync();

        // All pages we're testing require authentication, so log in first
        await LoginAsync();
    }

    // Runs after each individual test: close the context to discard cookies/state
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    // ---------------------------------------------------------------------------
    // Login helper
    // ---------------------------------------------------------------------------

    // Drives the app through Auth0's Universal Login page.
    // Credentials come from environment variables so nothing sensitive is in code.
    private async Task LoginAsync()
    {
        var email = Environment.GetEnvironmentVariable("TEST_AUTH0_EMAIL")
            ?? throw new InvalidOperationException(
                "Set the TEST_AUTH0_EMAIL environment variable before running E2E tests.");

        var password = Environment.GetEnvironmentVariable("TEST_AUTH0_PASSWORD")
            ?? throw new InvalidOperationException(
                "Set the TEST_AUTH0_PASSWORD environment variable before running E2E tests.");

        // Hitting /Account/Login triggers an OAuth redirect to auth0.com
        await _page.GotoAsync($"{BaseUrl}/Account/Login");

        // Wait for Auth0's login form to appear (the input name is set by Auth0)
        await _page.WaitForSelectorAsync("input[name='username']");

        // Fill in the email and password fields
        await _page.FillAsync("input[name='username']", email);
        await _page.FillAsync("input[name='password']", password);

        // Submit the form — Auth0 will redirect us back to the app when done
        await _page.ClickAsync("button[type='submit']");

        // Wait until the URL is back on our app domain, meaning login succeeded
        await _page.WaitForURLAsync($"{BaseUrl}/**");
    }

    // ---------------------------------------------------------------------------
    // Test 1: Transactions page loads
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task TransactionsPage_Loads_ShowsHeadingAndAddButton()
    {
        // Go to the Transactions list
        await _page.GotoAsync($"{BaseUrl}/Transactions");

        // The page should show the "All Transactions" heading
        await Expect(
            _page.GetByRole(AriaRole.Heading, new() { Name = "All Transactions" })
        ).ToBeVisibleAsync();

        // There should be an "+ Add Transaction" link at the top of the page
        await Expect(
            _page.GetByRole(AriaRole.Link, new() { Name = "+ Add Transaction" })
        ).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------------------------
    // Test 2: Add a new transaction via the form, verify it appears in the list
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task CreateTransaction_ValidForm_AppearsInList()
    {
        // Add a timestamp so this description is unique and easy to find in the list
        var description = $"E2E Coffee Run {DateTime.Now:HHmmss}";

        // Open the Create Transaction form
        await _page.GotoAsync($"{BaseUrl}/Transactions/Create");

        // Fill in the required fields.
        // The input names (Description, Amount, Date) come from the model property names
        // because ASP.NET's asp-for tag helper uses them as the HTML name attribute.
        await _page.FillAsync("input[name='Description']", description);
        await _page.FillAsync("input[name='Amount']", "4.75");

        // HTML date inputs expect the value in yyyy-MM-dd format
        await _page.FillAsync("input[name='Date']", DateTime.Today.ToString("yyyy-MM-dd"));

        // CategoryId is optional (nullable in the model), so we leave the
        // dropdown on "-- Select a Category --" and do not select anything

        // Click the Save Transaction button to submit the form
        await _page.ClickAsync("button[type='submit']");

        // After a successful save, the controller redirects to the Transactions list
        await _page.WaitForURLAsync($"{BaseUrl}/Transactions");

        // The description we entered should now appear somewhere in the list
        await Expect(_page.GetByText(description)).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------------------------
    // Test 3: Add a new category via the form, verify it appears in the list
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task CreateCategory_ValidForm_AppearsInList()
    {
        // Unique name so we can tell this row apart from pre-existing categories
        var categoryName = $"E2E Groceries {DateTime.Now:HHmmss}";

        // Open the Create Category form
        await _page.GotoAsync($"{BaseUrl}/Categories/Create");

        // Fill in the Name field (the only required field on this form)
        await _page.FillAsync("input[name='Name']", categoryName);

        // Click the Save Category button to submit the form
        await _page.ClickAsync("button[type='submit']");

        // After a successful save, the controller redirects to the Categories list
        await _page.WaitForURLAsync($"{BaseUrl}/Categories");

        // The category name should now appear somewhere in the list
        await Expect(_page.GetByText(categoryName)).ToBeVisibleAsync();
    }

    // ---------------------------------------------------------------------------
    // Test 4: Dashboard loads and the Total Spending card is visible
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task Dashboard_TotalSpendingCard_IsVisible()
    {
        // Navigate to the Dashboard page
        await _page.GotoAsync($"{BaseUrl}/Home/Dashboard");

        // The page contains a Bootstrap card with the title "Total Spending"
        await Expect(_page.GetByText("Total Spending")).ToBeVisibleAsync();
    }
}
