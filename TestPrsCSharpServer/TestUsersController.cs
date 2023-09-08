using Microsoft.AspNetCore.Mvc;

using PrsCSharpServer.Controllers;
using PrsCSharpServer.Data;
using PrsCSharpServer.Models;

namespace TestPrsCSharpServer;

public class TestUsersController {

    public readonly UsersController usrCtrl;

    public TestUsersController() {
        usrCtrl = new UsersController(new PrsDbContext());
    }

    [Fact]
    public async void TestLogin() {
        var user = await usrCtrl.GetUserLogin("sa", "sa");
        Assert.IsType<ActionResult<User>?>(user);
        var user2 = await usrCtrl.GetUserLogin("sa", "sax");
        Assert.IsNotType<ActionResult<User>?>(user2);
    }
}