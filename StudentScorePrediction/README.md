# Student Score Prediction System

یک پروژه کامل و حرفه‌ای برای پیش‌بینی نمره نهایی دانش‌آموزان با استفاده از ML.NET و Blazor.

## 🎯 هدف پروژه

این سیستم بر اساس اطلاعات یک دانش‌آموز (سن، جنسیت، ساعات مطالعه، نرخ حضور و ...) نمره نهایی او را پیش‌بینی می‌کند.

## 🛠 تکنولوژی‌ها

### Backend
- C# / .NET 10
- ASP.NET Core Web API
- ML.NET (Machine Learning)
- Entity Framework Core
- SQL Server

### Frontend
- Blazor WebAssembly
- MudBlazor (UI Component Library)

### Documentation
- Swagger / OpenAPI

## 🏗 معماری پروژه

پروژه از الگوی Clean Architecture پیروی می‌کند:

```
StudentScorePrediction/
├── src/
│   ├── StudentScorePrediction.Domain/          # Entities, Enums
│   ├── StudentScorePrediction.Application/     # DTOs, Interfaces, Services
│   ├── StudentScorePrediction.Infrastructure/  # EF Core, Repositories
│   ├── StudentScorePrediction.ML/              # ML.NET, Dataset Generator
│   ├── StudentScorePrediction.Api/             # ASP.NET Core Web API
│   └── StudentScorePrediction.Client/          # Blazor WebAssembly
└── tests/
    └── StudentScorePrediction.Tests/           # Unit Tests
```

## 📊 Featureهای مدل

### ورودی‌ها (Features):
- Age (سن)
- Gender (جنسیت)
- StudyHours (ساعات مطالعه)
- AttendanceRate (نرخ حضور)
- HomeworkCompletionRate (نرخ تکمیل تکالیف)
- PreviousAverage (میانگین نمرات قبلی)
- PreviousExamScore (نمره امتحان قبلی)
- MidtermScore (نمره میان‌ترم)
- AbsenceDays (تعداد روزهای غیبت)
- SleepHours (ساعات خواب)
- ClassParticipation (مشارکت در کلاس)
- MobileUsageHours (ساعات استفاده از موبایل)
- PracticeTestCount (تعداد آزمون‌های تمرینی)

### خروجی (Label):
- FinalScore (نمره نهایی - بین 0 تا 20)

## 🚀 راهنمای اجرا

### پیش‌نیازها
1. نصب .NET 10 SDK
2. نصب SQL Server
3. نصب Visual Studio 2022 یا VS Code

### مراحل اجرا

#### 1. Restore کردن پکیج‌ها
```bash
cd StudentScorePrediction
dotnet restore
```

#### 2. Build کردن پروژه
```bash
dotnet build
```

#### 3. تنظیم Connection String
فایل `appsettings.json` را باز کرده و Connection String را تنظیم کنید:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StudentScorePrediction;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### 4. ایجاد Database
```bash
cd src/StudentScorePrediction.Api
dotnet ef database update
```

اگر EF Core CLI نصب نیست:
```bash
dotnet tool install --global dotnet-ef
```

#### 5. اجرای API
```bash
dotnet run
```

API روی https://localhost:7001 اجرا می‌شود.

#### 6. اجرای Blazor Client (در ترمینال دیگر)
```bash
cd src/StudentScorePrediction.Client
dotnet run
```

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/students | دریافت لیست دانش‌آموزان |
| POST | /api/students | ایجاد دانش‌آموز جدید |
| GET | /api/predictions | دریافت لیست پیش‌بینی‌ها |
| POST | /api/predictions | پیش‌بینی نمره |
| POST | /api/training/start | شروع آموزش مدل |
| GET | /api/training/status | وضعیت آموزش |
| GET | /api/models | دریافت لیست مدل‌ها |
| POST | /api/models/{id}/activate | فعال‌سازی مدل |
| POST | /api/dataset/generate | تولید Dataset |
| GET | /api/dataset/statistics | آمار Dataset |

## 🤖 Machine Learning

### الگوریتم‌های پشتیبانی شده:
1. **SDCA Regression** - سریع و مناسب برای داده‌های بزرگ
2. **FastTree Regression** - دقت بالا، زمان آموزش متوسط
3. **FastForest Regression** - مقاوم در برابر Overfitting

### Metricهای ارزیابی:
- **MAE** (Mean Absolute Error) - میانگین خطای مطلق
- **MSE** (Mean Squared Error) - میانگین خطای مربعی
- **RMSE** (Root Mean Squared Error) - جذر میانگین خطای مربعی
- **R²** (Coefficient of Determination) - ضریب تعیین

## 📁 Dataset Generation

سیستم قابلیت تولید Datasetهای بزرگ را دارد:
- حداقل: 1,000 رکورد
- پیشنهادی: 100,000 رکورد
- حداکثر: 1,000,000 رکورد

داده‌ها به صورت واقع‌گرایانه تولید می‌شوند با روابط منطقی بین Featureها.

## 🧪 Testing

برای اجرای تست‌ها:
```bash
cd tests/StudentScorePrediction.Tests
dotnet test
```

## 📝 مفاهیم Machine Learning

### اصطلاحات مهم:

| اصطلاح | معنی | توضیح |
|--------|------|-------|
| **Feature** | ویژگی | ورودی‌های مدل (مثل سن، ساعات مطالعه) |
| **Label** | برچسب | مقداری که مدل پیش‌بینی می‌کند (نمره نهایی) |
| **Training** | آموزش | فرآیند یادگیری مدل از داده‌ها |
| **Prediction** | پیش‌بینی | استفاده از مدل برای تخمین مقدار جدید |
| **Regression** | رگرسیون | نوعی مسئله که خروجی عددی است |
| **Overfitting** | بیش‌برازش | وقتی مدل بیش از حد به داده آموزشی وابسته است |
| **Dataset** | مجموعه داده | مجموعه‌ای از نمونه‌های آموزشی |

## 🎨 UI Pages

- **/** - Dashboard با نمودارها و آمار
- **/students** - مدیریت دانش‌آموزان
- **/prediction** - صفحه پیش‌بینی نمره
- **/predictions** - تاریخچه پیش‌بینی‌ها
- **/training** - آموزش مدل
- **/models** - مدیریت مدل‌ها
- **/dataset** - مدیریت Dataset

## 🔧 Troubleshooting

### خطای اتصال به Database
- مطمئن شوید SQL Server در حال اجرا است
- Connection String را بررسی کنید
- دسترسی کاربر را بررسی کنید

### خطای ML.NET
- مطمئن شوید Dataset وجود دارد
- حجم RAM کافی باشد (برای Datasetهای بزرگ)

## 📄 License

این پروژه برای اهداف آموزشی ایجاد شده است.

## 👨‍💻 Author

پروژه دانشجویی - یادگیری Machine Learning با C#
