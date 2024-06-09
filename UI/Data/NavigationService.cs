namespace UI.Data;

public struct NavigationLink
{
    public string Icon { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
}

public class NavigationService
{
    public event Action? OnSetNavigation;
    public List<NavigationLink> Links { get; set; }
    private readonly List<NavigationLink> _loggedInLinks = new List<NavigationLink>
    {
        new NavigationLink { Icon = "garage_door", Name = "Deposits" },
        new NavigationLink { Icon = "book", Name = "Bookings" },
        new NavigationLink() { Icon = "sell", Name = "Promotions", IsAdmin = true},
        new NavigationLink { Icon = "logout", Name = "Logout" }
    };
    private readonly List<NavigationLink> _loggedOutLinks = new List<NavigationLink>
    {
        new NavigationLink { Icon = "login", Name = "Login" },
        new NavigationLink { Icon = "passkey", Name = "Register" }
    };

    public NavigationService()
    {
        Links = _loggedOutLinks;
    }

    public void SetLoggedInLinks()
    {
        Links = _loggedInLinks;
        OnSetNavigation?.Invoke();
    }

    public void SetLoggedOutLinks()
    {
        // login register
        Links = _loggedOutLinks;
        OnSetNavigation?.Invoke();
    }
}