using Microsoft.Playwright;

namespace BudgetTracker.Tests.E2E;

// This fixture is shared across all tests in BudgetTrackerE2ETests.
// xUnit creates it once, runs all tests, then disposes it — so the browser
// process starts and stops only once per test run, not once per test.
public class PlaywrightFixture : IAsyncLifetime
{
    // Expose these so the test class can create new browser contexts/pages
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    // Called once before the first test — start Playwright and open the browser
    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        // Run headless in CI (where the CI environment variable is set), headed locally
        // so you can watch the browser while developing or debugging tests.
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = isCI
        });
    }

    // Called once after the last test — shut everything down cleanly
    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        Playwright.Dispose();
    }
}
