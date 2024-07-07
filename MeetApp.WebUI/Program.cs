using MeetApp.Business;
using MeetApp.Data;
using MeetApp.Infrastructure.Commons.Concretes;
using MeetApp.Infrastructure.Services.Abstracts;
using MeetApp.Infrastructure.Services.Concretes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SignalR;

public class Program
{
    internal static string[] policies = null;
    public static void Main(string[] args)
    {
        //ReadAllPolicies();

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews(cfg =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

            cfg.Filters.Add(new AuthorizeFilter(policy));

        });


        builder.Services.AddRouting(cfg => cfg.LowercaseUrls = true);

        builder.Services.Configure<EmailOptions>(cfg =>
        {
            builder.Configuration.GetSection("emailAccount").Bind(cfg);
        });
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
        //builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        //builder.Services.AddScoped<IClaimsTransformation, AppClaimProvider>();
        DataServiceInjection.InstallDataServices(builder.Services, builder.Configuration);

        builder.Services.AddMediatR(cfg =>
        {

            cfg.RegisterServicesFromAssembly(typeof(IBusinessServices).Assembly);

        });

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(cfg =>
            {
                cfg.LoginPath = "/signin.html";
                cfg.AccessDeniedPath = "/access-denied.html";
                cfg.ExpireTimeSpan = TimeSpan.FromMinutes(10);

                cfg.Cookie.Name = "AZMeet";
                cfg.Cookie.HttpOnly = true;
            });
        //builder.Services.AddAuthorization(cfg =>
        //{
        //    foreach (var policyName in policies)
        //    {
        //        cfg.AddPolicy(policyName, p => p.RequireAssertion(handler =>
        //        {
        //            return handler.User.IsInRole("superadmin") || handler.User.HasClaim(policyName, "1");
        //        }));
        //    }
        //});

        builder.Services.Configure<IdentityOptions>(cfg =>
        {

            cfg.User.RequireUniqueEmail = true;
            cfg.Password.RequireUppercase = false;
            cfg.Password.RequireLowercase = false;
            cfg.Password.RequireDigit = false;
            cfg.Password.RequiredUniqueChars = 1;
            cfg.Password.RequiredLength = 3;
            cfg.Password.RequireNonAlphanumeric = false;

            cfg.SignIn.RequireConfirmedEmail = true;
            cfg.Lockout.AllowedForNewUsers = true;
            cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            cfg.Lockout.MaxFailedAccessAttempts = 3;

        });




        var app = builder.Build();


        app.UseStaticFiles();
        app.UseRouting();


        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(cfg =>
        {
            //cfg.MapControllerRoute(
            //      name: "areas",
            //      pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            //    );

            cfg.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");

        });

        app.Run();

        app.MapHub<ChatHub>("/chathub");

        app.Run();
    }

    //private static void ReadAllPolicies()
    //{
    //    var types = typeof(Program).Assembly.GetTypes();

    //    policies = types
    //        .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && t.IsDefined(typeof(AuthorizeAttribute), true))
    //        .SelectMany(t => t.GetCustomAttributes<AuthorizeAttribute>())
    //        .Union(
    //        types
    //        .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
    //        .SelectMany(type => type.GetMethods())
    //        .Where(method => method.IsPublic
    //         && !method.IsDefined(typeof(NonActionAttribute), true)
    //         && method.IsDefined(typeof(AuthorizeAttribute), true))
    //         .SelectMany(t => t.GetCustomAttributes<AuthorizeAttribute>())
    //        )
    //        .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
    //        .SelectMany(a => a.Policy.Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries))
    //        .Distinct()
    //        .ToArray();
    //}
}




public static class Users
{
    public static IDictionary<string, string> list = new Dictionary<string, string>();

}
public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;


    public ChatHub(
        ILogger<ChatHub> logger)


    {
        _logger = logger;

    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"New connection established : {Context.ConnectionId}");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        Clients.All.SendAsync("user-disconnected", Users.list[Context.ConnectionId]);
        return base.OnDisconnectedAsync(exception);
    }
    public async Task JoinRoom(string roomId, string userId)
    {
        Users.list.Add(Context.ConnectionId, userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("user-connected", userId);
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", "_userService.GetCurrentUserFullName()", message); ;
    }
}


//public class ChatHub : Hub
//{
//    private static ConcurrentBag<(string User, string Message)> Messages = new ConcurrentBag<(string, string)>();

//    private static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();

//    public override async Task OnConnectedAsync()
//    {
//        // Send all previous messages to the newly connected client
//        foreach (var message in Messages)
//        {
//            await Clients.Caller.SendAsync("ReceiveMessage", message.User, message.Message);
//        }
//        ConnectedUsers.Add(Context.ConnectionId, Context.User.Identity.Name);

//        await base.OnConnectedAsync();
//    }

//    public async Task SendMessage(string message)
//    {
//        var user = Context.User.Identity.Name ?? "Anonymous";
//        var messageTuple = (User: user, Message: message);

//        // Add the message to the list
//        Messages.Add(messageTuple);
//        await Clients.All.SendAsync("ReceiveMessage", user, message);

//    }
//    public override Task OnDisconnectedAsync(Exception exception)
//    {
//        ConnectedUsers.Remove(Context.ConnectionId);
//        return base.OnDisconnectedAsync(exception);
//    }



//    public async Task SendSignal(string peerId, string data)
//    {
//        await Clients.Others.SendAsync("ReceiveSignal", Context.ConnectionId, peerId, data);
//    }

//    public IEnumerable<string> GetConnectedPeers()
//    {
//        return ConnectedUsers.Keys;
//    }
//}

