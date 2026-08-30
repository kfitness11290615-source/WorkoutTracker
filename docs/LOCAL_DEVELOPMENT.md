# ローカル開発 & 実機テストガイド

本ドキュメントでは、WorkoutTracker のローカル開発環境のセットアップ手順、スマートフォン実機でのデバッグ・テスト方法、およびデータベースマイグレーション手順について解説します。

---

## 1. 開発環境の準備

### 必要なツール
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Visual Studio 2022 (v17.12以降)](https://visualstudio.microsoft.com/) または [Visual Studio Code](https://code.visualstudio.com/) (C# Dev Kit 拡張機能)
* [Git](https://git-scm.com/) (Git LFS 対応)

---

## 2. 初回セットアップ手順

### ステップ 1: リポジトリのクローン
```bash
git clone https://github.com/your-username/WorkoutTracker.git
cd WorkoutTracker
```

### ステップ 2: 設定ファイル (`appsettings.json`) の準備
リポジトリに含まれる `appsettings.Example.json` をコピーして、ローカル専用の `appsettings.json` を作成します。

```bash
# Windows PowerShell の場合
Copy-Item .\WorkoutTracker\appsettings.Example.json .\WorkoutTracker\appsettings.json
```

作成した `WorkoutTracker/appsettings.json` を開き、任意のユーザー名・パスワードを設定します：
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "BasicAuth": {
    "Username": "dev",
    "Password": "password123"
  }
}
```

> [!NOTE]
> `appsettings.json` は `.gitignore` に登録されているため、Git にコミットされる心配はありません。

### ステップ 3: アプリのビルド & 実行
```bash
cd WorkoutTracker
dotnet run
```

ブラウザで `http://localhost:5221`（または起動ログに表示されたURL）を開きます。
初回起動時に SQLite データベース（`workout.db`）が自動生成され、シードデータ（部位・種目・デフォルト週プロファイル）が自動投入されます。

---

## 3. スマートフォン実機でのローカルテスト手順

ジムでの片手操作感やモバイルブラウザ（Safari/Chrome）でのレイアウトをPCと同じWi-Fi環境下でテストできます。

### 3.1 launchSettings.json のバインド設定
`WorkoutTracker/Properties/launchSettings.json` にて、外部アクセスを受け付けるよう `0.0.0.0` にバインドされています：

```json
"applicationUrl": "http://0.0.0.0:5221"
```

### 3.2 PCのローカルIPアドレスの確認
PowerShell で以下を実行して IPv4 アドレスを確認します：
```powershell
ipconfig
# 例: 192.168.1.10
```

### 3.3 スマホからアクセス
1. スマホを PC と同じ Wi-Fi に接続します。
2. スマホのブラウザ（Safari / Chrome）のアドレスバーに `http://<PCのIPアドレス>:5221`（例: `http://192.168.1.10:5221`）を入力します。
3. Basic認証ダイアログが表示されたら、`appsettings.json` に設定した認証情報を入力してログインします。

---

## 4. データベース & EF Core マイグレーション手順

### Entity Framework Core ツールのインストール（未導入の場合）
```bash
dotnet tool install --global dotnet-ef
```

### マイグレーションの追加（モデル変更時）
```bash
dotnet ef migrations add <MigrationName> --project WorkoutTracker
```

### データベースの更新
アプリ起動時に `Program.cs` の `db.Database.Migrate()` で自動適用されますが、手動で適用する場合は以下を実行します：
```bash
dotnet ef database update --project WorkoutTracker
```
