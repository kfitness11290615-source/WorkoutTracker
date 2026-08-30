using WorkoutTracker.Components;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Services;
using WorkoutTracker.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ここで、アプリ内で使う「サービス（機能）」を登録します。

// 1. Blazorという画面を描画するための基本サービスを登録
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. データベース(SQLite)に接続するためのサービスを登録
var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
var dataDir = isAzure ? @"D:\home\Data" : builder.Environment.ContentRootPath;
if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

var dbPath = Path.Combine(dataDir, "workout.db");

if (isAzure && !File.Exists(dbPath))
{
    var deployedDbPath = Path.Combine(builder.Environment.ContentRootPath, "workout.db");
    if (File.Exists(deployedDbPath))
    {
        File.Copy(deployedDbPath, dbPath);
    }
}

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 3. トレーニング推奨重量を計算する自作のサービスを登録
// AddScoped と書くことで、1ユーザーのアクセスごとにこの計算サービスが生成されて使われます
builder.Services.AddScoped<TrainingRecommendationService>();
builder.Services.AddHostedService<DbBackupService>();

// 設定が完了したのでアプリをビルド（構築）します
var app = builder.Build();

// アプリが起動した直後に実行される処理
// データベースの中身（テーブル構造など）がまだ無ければ、自動的に作成（Migrate）します。
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbContextFactory.CreateDbContext();
    // データベースの自動作成・マイグレーション
    db.Database.Migrate();
}

// 通信エラー時の対応や、セキュリティ関連の基本設定
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.UseMiddleware<BasicAuthMiddleware>();

app.MapStaticAssets();

// アプリケーションのエントリーポイント（どの画面を最初に描画するか）を指定
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// アプリケーションを起動し、Webブラウザからのアクセスを待ち受けます
app.Run();
