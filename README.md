# GMedia

## EN

### Made by Georgi Minchev Yordanov - georgimyordanov07@gmail.com

### Configuration
1. Change the connection string in `appsettings.json` to the appropriate one
2. Run the `add-migration "Appropriate migration name"` command in the Package Manager Console
3. Then run the `update-database` command in the Package Manager Console

### Roadmap
Each feature and where in the project it's implemented:

- All models – `Models/User.cs` | `Models/Friendship.cs` | `Models/Invitation.cs` | `Models/Post.cs` | 
- Creating and seeding initial data (Posts, Users, Friendships, and Invitations) – `Seeder.cs`
- Registration after receiving an invitation – `Areas/Identity/Pages/Account/Register.cshtml | Register.cshtml.cs`
- Login - `Areas/Identity/Pages/Account/Login.cshtml | Login.cshtml.cs`
- Logout (after confirmation) - `Views/Shared/_LoginPartial.cshtml`
- Invite user via email (with generated access code) – `Views/Invitations/Create.cshtml` | `Controllers/InvitationsController.cs`
- Posts timeline – `HomeController.cs` | `Views/Home/Index.cshtml`
- All Friends – `Areas/Identity/Pages/Account/Manage/Index.cshtml | Index.cshtml.cs`
- View Specific User - `Controllers/UsersController.cs` | `Views/Users/Details.cshtml`

## БГ

### Направено от Георги Минчев Йорданов - georgimyordanov07@gmail.com

### Конфигурация
1. Променете стойността на връзката към базата данни в appsettings.json с подходящата за вашата среда.
2. Изпълнете командата add-migration "Име на миграцията" в Package Manager Console.
3. След това изпълнете командата update-database, за да се създаде или обнови базата данни.

### Карта на навигация
Всяка функционалност и къде в проекта е реализирана:

- Всички модели – `Models/User.cs` | `Models/Friendship.cs` | `Models/Invitation.cs` | `Models/Post.cs`
- Създаване и начално зареждане на данни (Публикации, Потребители, Приятелства и Покани) – `Seeder.cs`
- Регистрация след получаване на покана – `Areas/Identity/Pages/Account/Register.cshtml | Register.cshtml.cs`
- Вход – `Areas/Identity/Pages/Account/Login.cshtml | Login.cshtml.cs`
- Изход (със потвърждение) – `Views/Shared/_LoginPartial.cshtml`
- Изпращане на покана по имейл (с генериран код) – `Views/Invitations/Create.cshtml` | `Controllers/InvitationsController.cs`
- Времева линия с публикации – `HomeController.cs` | `Views/Home/Index.cshtml`
- Всички приятели – `Areas/Identity/Pages/Account/Manage/Index.cshtml | Index.cshtml.cs`
- Преглед на конкретен потребител – `Controllers/UsersController.cs` | `Views/Users/Details.cshtml`
