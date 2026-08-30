# Microsoft Azure デプロイ & クラウド運用ガイド

WorkoutTracker を **Microsoft Azure App Service** にデプロイし、24時間スマートフォンやPCから安全にアクセスできるようにするための完全ガイドです。

---

## 1. 前提条件とアーキテクチャ

WorkoutTracker は SQLite データベースを利用しているため、**Windows App Service**（無料プラン `Free F1` 対応）へのホスティングに最適化されています。

* **永続ストレージ**: Azure App Service (Windows) では、`D:\home\Data` ディレクトリがアプリの再起動や再デプロイでも消えない永続化ストレージ領域となります。
* **起動時DB自動配置**: アプリ初回起動時、`D:\home\Data\workout.db` が存在しない場合は初期DBが自動コピーされ、マイグレーションが自動実行されます。

```
Azure App Service (Windows)
├── C:\home\site\wwwroot\          # アプリ本体 (デプロイ時に上書きされる領域)
└── D:\home\Data\                  # 永続ストレージ (保護領域)
    ├── workout.db                 # 実運用 SQLite データベース
    └── Backups\                   # 起動時自動バックアップ (最大14世代)
        ├── workout_20260826_132501.db
        └── ...
```

---

## 2. Azure Web アプリの作成手順

### 2.1 Azure Portal での作成
1. [Azure Portal](https://portal.azure.com/) にサインインします。
2. 検索バーで「**App Services**」を検索し、「＋ 作成」>「Web アプリ」をクリックします。
3. 以下の項目を設定します：

| 設定項目 | 推奨設定値 | 備考 |
|:---|:---|:---|
| **サブスクリプション** | ご自身のサブスクリプション | |
| **リソース グループ** | 新規作成 (例: `workout-rg`) | 任意 |
| **名前 (Web アプリ名)** | 任意の一意な名前 (例: `my-workout-tracker`) | `https://<名前>.azurewebsites.net` になります |
| **公開** | `コード` | |
| **ランタイム スタック** | `.NET 9 (STS)` | |
| **オペレーティング システム** | **`Windows`** | ※SQLiteのファイルロックと永続化のため必ず Windows を選択 |
| **地域** | `Japan East` (東日本) | 最寄りのリージョン |
| **価格プラン** | **`Free F1`** (無料枠) | 無料で運用可能 |

4. 「確認および作成」をクリックし、デプロイ完了を待ちます。

---

## 3. 認証情報（Basic認証）の設定

GitHub リポジトリにパスワードをコミットせずに、Azure 上で安全に認証情報を管理するために **「環境変数（アプリケーション設定）」** を利用します。

1. Azure Portal で作成した Web アプリを開きます。
2. 左メニューの「設定」>「**環境変数**」を開きます。
3. 「アプリ設定」タブで「＋ 追加」をクリックし、以下の2つのキーを設定します：

| 名前 (キー) | 値 (例) | 説明 |
|:---|:---|:---|
| `BasicAuth__Username` | `fit` | ログイン用ユーザー名 (任意) |
| `BasicAuth__Password` | `YourSecurePassword!` | ログイン用パスワード (任意) |

> [!NOTE]
> ASP.NET Core の設定階層は環境変数ではアンダースコア2つ（`__`）で表現されます。`BasicAuth:Username` $\rightarrow$ `BasicAuth__Username`

4. 「適用」を押して保存します。

---

## 4. アプリケーションのデプロイ

### 方法 A: VS Code の Azure 拡張機能を使用（推奨）
1. VS Code に **Azure App Service** 拡張機能（`ms-azuretools.vscode-azureappservice`）をインストールします。
2. 左の Azure アイコンからサインインします。
3. リソース一覧から対象の App Service を右クリックし、「**Deploy to Web App...**」を選択します。
4. `WorkoutTracker` フォルダを選択してデプロイを実行します。

### 方法 B: Visual Studio 2022 を使用
1. Visual Studio で `WorkoutTracker.sln` を開きます。
2. ソリューション エクスプローラーで `WorkoutTracker` プロジェクトを右クリックし、「**発行 (Publish...)**」を選択します。
3. ターゲットに「**Azure**」>「**Azure App Service (Windows)**」を選択し、作成した Web アプリと連携します。
4. 「発行」ボタンを押すと、ビルド・パッケージング・アップロードが自動実行されます。

---

## 5. データベースのバックアップとリストア

* **自動バックアップ**: アプリが再起動されるたびに、`D:\home\Data\Backups` 配下に最新の DB がバックアップされます。
* **リストア（復元）**: 万が一データを元に戻したい場合は、Kudu（高度な管理ツール: `https://<名前>.scm.azurewebsites.net/DebugConsole`）から `D:\home\Data\Backups` 内のバックアップファイルを `D:\home\Data\workout.db` に上書きコピーしてください。
