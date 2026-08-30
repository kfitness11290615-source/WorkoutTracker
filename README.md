# 🏋️‍♂️ WorkoutTracker

> **6週ピリオダイゼーション理論と推定1RM (e1RM) に基づく、モバイルファースト筋力トレーニング記録・管理 Web アプリケーション**

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Interactive%20Server-512BD4?style=flat&logo=blazor)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4?style=flat)](https://learn.microsoft.com/ef/core/)
[![SQLite](https://img.shields.io/badge/SQLite-Embedded-003B57?style=flat&logo=sqlite)](https://www.sqlite.org/)
[![Azure App Service](https://img.shields.io/badge/Azure-App%20Service-0078D4?style=flat&logo=microsoftazure)](https://azure.microsoft.com/services/app-service/)
[![License](https://img.shields.io/badge/License-Non--Commercial-red?style=flat)](LICENSE)

---

## 📌 概要

**WorkoutTracker** は、筋肥大・筋力向上を効率的に目指すトレーニーのためのWebアプリケーションです。
単なる重量・レップ数のメモにとどまらず、**6週間のピリオダイゼーション（周期化）理論** と **RPE（主観的運動強度）に基づく推定1RM (e1RM)** を用いて、毎回のトレーニングで「何kgで何レップを目指すべきか」をリアルタイムに自動提示します。

ジムでのインターバル中にもスムーズに入力できるよう、片手操作に最適化したダークテーマのモバイルUIを採用しています。

---

## ✨ 主な機能

### 1. 🏋️‍♂️ トレーニング記録 & リアルタイム重量推奨
* **6週サイクルに連動した目標値自動提示**:
  * サイクル内の現在週（漸進期 / 高強度期 / AMRAP評価 / デロード）を部位ごとのトレーニング頻度から自動計算。
  * 現在の週フェーズに応じた目標 RPE・セット数・レップ範囲を自動表示。
* **前回実績（全セット）の参照表示**:
  * 前回の同一種目の全セット実績（重量・レップ・RPE）を一目で確認。
* **次セット重量のリアルタイム動的再計算**:
  * 1セット目の結果（重量・レップ・RPE）を入力すると、その場で算出した最新の e1RM に基づいて次セットの最適推奨重量を自動計算。
* **AMRAP週の自動記録更新**:
  * Week 5 (AMRAP評価週) で自己ベストの e1RM を記録すると、次サイクル向けの基準重量が自動更新。

### 2. 📈 進捗可視化 (AMRAP & e1RM グラフ)
* **Chart.js 連携グラフ**:
  * AMRAP対象種目の推定1RM（e1RM）推移を折れ線グラフで可視化。
  * 開始時 e1RM、最新 e1RM、過去最大 e1RM のスタッツを自動集計。

### 3. 📅 カレンダー機能
* **月間トレーニング履歴**:
  * トレーニングを実施した日が一目でわかるドット付き月間カレンダー。
  * 任意の日付をタップして過去の記録の閲覧や過去日の記録入力が可能。

### 4. ⚙️ 柔軟なカスタマイズ設定
* **部位ごとの週頻度設定**:
  * 部位ごと（胸、背中、肩、脚、二頭、三頭、腹）の週あたりトレーニング日数を設定し、サイクル進行速度を個別に調整。
* **2層のプロファイル設定 (Global / Custom)**:
  * コンパウンド種目（AMRAP追跡）とアイソレーション種目の全体デフォルトプロファイル。
  * 種目個別のカスタム週プロファイル（Week 1〜6 の目標RPE・セット数・レップ範囲）を設定可能。
* **種目の自由な追加・編集・削除**:
  * 重量刻み幅（例: 2.5kg, 1.0kg, 0.5kg）、基準重量、目標レップ範囲を自在に管理。

### 5. 🔒 セキュリティ & クラウド運用対応
* **Basic 認証**: 不正アクセスを防ぐ認証ミドルウェアを標準搭載。
* **自動世代バックアップ**: アプリ起動時に SQLite データベースを自動バックアップ（最新14世代を自動保持）。
* **Azure App Service (Windows) 永続ストレージ対応**: デプロイや再起動でデータが消えない `D:\home\Data` ストレージ構成。

---

## 📐 計算アルゴリズムと理論

### 推定1RM (e1RM) 計算式
$$\text{e1RM} = \text{Weight} \times \left(1 + \frac{\text{Rep} + 10 - \text{RPE}}{30}\right)$$

### 推奨重量の逆算式
$$\text{RecommendedWeight} = \text{RoundToIncrement}\left( \frac{\text{e1RM}}{1 + \frac{\text{TargetMidRep} + 10 - \text{TargetRPE}}{30}}, \text{WeightIncrement} \right)$$

### 6週ピリオダイゼーションフェーズ
| 週 | フェーズ | 目的 | コンパウンド目標RPE | アイソレーション目標RPE | セット数 |
|:---:|:---|:---|:---:|:---:|:---|
| **W1** | **漸進①** | 導入・フォーム定着 | 7.0 | 7.0 | 基本セット数 |
| **W2** | **漸進②** | 負荷増加 | 7.5 | 7.5 | ボリュームUP (+1〜2 sets) |
| **W3** | **漸進③** | 負荷ピーク進行 | 8.0 | 8.0 | ボリューム最大 (最大5 sets) |
| **W4** | **高強度** | 神経系刺激・強度重視 | 9.0 | 8.0 | セット数を絞り強度重視 |
| **W5** | **AMRAP評価** | 最大能力テスト | 9.5 (MAX挑戦) | 7.5 | 2 sets (e1RM自動更新) |
| **W6** | **疲労抜き** | デロード（積極的休養） | 7.0 | 7.5 | 2 sets (疲労回復) |

---

## 🛠️ 技術スタック

* **フレームワーク**: [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) (C# 13)
* **フロントエンド**: ASP.NET Core Blazor Interactive Server
* **ORM / データベース**: Entity Framework Core 9, SQLite
* **データ可視化**: [Chart.js](https://www.chartjs.org/) (JavaScript Interop)
* **スタイリング**: スコープ付き CSS3 (モバイルファースト・ダークテーマ)
* **ホスティング / インフラ**: Microsoft Azure App Service (Windows / Free F1 対応)

---

## 🚀 クイックスタート (ローカル実行)

### 1. 前提条件
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### 2. リポジトリのクローン
```bash
git clone https://github.com/your-username/WorkoutTracker.git
cd WorkoutTracker
```

### 3. 設定ファイルの作成
テンプレートからローカル用の `appsettings.json` を作成します：
```bash
# Windows PowerShell
Copy-Item .\WorkoutTracker\appsettings.Example.json .\WorkoutTracker\appsettings.json
```

`WorkoutTracker/appsettings.json` を開き、ログイン用ユーザー名・パスワードを設定します：
```json
{
  "BasicAuth": {
    "Username": "admin",
    "Password": "your_password"
  }
}
```

### 4. アプリケーションの実行
```bash
cd WorkoutTracker
dotnet run
```

ブラウザで `http://localhost:5221` を開き、設定した Basic 認証でログインします。

---

## 📚 ドキュメント一覧

詳しい設計や運用については `docs/` ディレクトリ内の各ガイドをご参照ください：

* 📘 [システムアーキテクチャ & 設計ドキュメント](docs/ARCHITECTURE.md)
  * コンポーネント構成図、データモデル仕様（ER図）、計算アルゴリズムの詳細
* ☁️ [Microsoft Azure デプロイ & クラウド運用ガイド](docs/AZURE_DEPLOYMENT.md)
  * Azure App Service へのデプロイ手順、環境変数での認証管理、DB永続化・バックアップ手順
* 📱 [ローカル開発 & スマホ実機テストガイド](docs/LOCAL_DEVELOPMENT.md)
  * ローカル環境構築、同一Wi-Fi下でのスマートフォン実機接続手順、EF Coreマイグレーション

---

## 📁 ディレクトリ構成

```
WorkoutTracker/
├── .github/                       # GitHub ワークフロー (CI/CD)
├── docs/                          # プロジェクト詳細ドキュメント
│   ├── ARCHITECTURE.md            # アーキテクチャ & 計算アルゴリズム解説
│   ├── AZURE_DEPLOYMENT.md        # Azure デプロイ手順書
│   └── LOCAL_DEVELOPMENT.md       # ローカル開発 & 実機テスト手順書
├── WorkoutTracker/                # アプリケーション本体
│   ├── Components/                # Blazor コンポーネント
│   │   ├── Layout/                # メインレイアウト & ボトムナビゲーション
│   │   └── Pages/                 # 各画面 (Home, TrainingLog, Progress, Calendar, Settings)
│   ├── Data/                      # DbContext & 初期シードデータ
│   ├── Middleware/                # Basic 認証ミドルウェア
│   ├── Migrations/                # EF Core データベースマイグレーション
│   ├── Models/                    # ドメインモデル (Exercise, Session, Record, Profile)
│   ├── Services/                  # ドメインサービス (E1RMCalculator, Recommendation, Backup)
│   ├── Properties/                # 起動・発行プロファイル
│   ├── wwwroot/                   # 静的アセット (CSS, JS, Favicon)
│   ├── appsettings.Example.json   # 公開用設定テンプレート
│   ├── Program.cs                 # エントリーポイント & DI設定
│   └── WorkoutTracker.csproj      # プロジェクト定義
├── .gitattributes                 # Git LFS & 改行コード設定
├── .gitignore                     # Git 除外設定
└── README.md                      # 本ドキュメント
```

---

## ⚠️ 免責事項・知的財産権に関する注記 (Disclaimer & IP Notice)

* **非公式・個人利用ツール**:
  本アプリケーションは、ピリオダイゼーション理論に基づくトレーニング記録・管理を効率化するために個人で開発された非公式ツールです。
* **原著作権の尊重**:
  本システムが参考としている筋肥大プログラム・ピリオダイゼーション手法に関する著作権、商標権、ノウハウ等の知的財産権は、すべて各原著作者（プログラム考案者様・権利者様）に帰属します。
* **元データの不保持・再配布禁止の遵守**:
  本リポジトリには、**再配布が禁止されている元データ（原著作者が提供するExcelファイル・教材データ等）は一切含まれていません。** 本アプリは汎用的な記録管理システムであり、種目名・重量等のデータは利用者が自己の責任において入力・管理する形式をとっています。

---

## 📄 ライセンス (License)

本プロジェクトは **非商用・個人利用限定ライセンス（[Non-Commercial & Personal Use License](LICENSE)）** の下で公開されています。

* **個人利用・学習・研究目的**: 個人のトレーニング管理、Blazor/.NET 学習の目的に限り、自由に利用・複製・改変していただけます。
* **商用利用の厳禁**: 本ソフトウェア（またはその派生物）を、有償サービスとしての提供、有料アプリへの組み込み、再販売、営利目的での利用を含む**一切の商用利用を固く禁止します**。

詳細な利用規約および法的条項については [LICENSE](LICENSE) ファイルをご確認ください。
