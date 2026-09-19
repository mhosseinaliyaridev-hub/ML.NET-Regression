# Student Score Prediction System

یک سیستم حرفه‌ای پیش‌بینی نمره دانش‌آموزان با استفاده از Machine Learning در C# و ML.NET

## 🎯 هدف پروژه

این پروژه یک سیستم کامل برای پیش‌بینی نمره نهایی دانش‌آموزان بر اساس ویژگی‌های مختلف تحصیلی و رفتاری است. هدف اصلی یادگیری عملی Machine Learning با C# بدون استفاده از Python می‌باشد.

## 🏗 معماری پروژه

پروژه از الگوی Clean Architecture پیروی می‌کند:

```
StudentScorePrediction/
├── src/
│   ├── StudentScorePrediction.Domain/          # Entities, Enums, Interfaces
│   ├── StudentScorePrediction.Application/     # DTOs, Services, Business Logic
│   ├── StudentScorePrediction.Infrastructure/  # EF Core, Repositories, Database
│   ├── StudentScorePrediction.ML/              # ML.NET, Dataset Generator
│   ├── StudentScorePrediction.Api/             # ASP.NET Core Web API
│   └── StudentScorePrediction.Client/          # Blazor WebAssembly + MudBlazor
└── tests/
    └── StudentScorePrediction.Tests/           # Unit Tests
```

## 🔧 تکنولوژی‌ها

### Backend
- **.NET 10**
- **ASP.NET Core Web API**
- **ML.NET** (Machine Learning)
- **Entity Framework Core**
- **SQL Server**

### Frontend
- **Blazor WebAssembly**
- **MudBlazor** (UI Components)

### Documentation
- **Swagger / OpenAPI**

## 📊 ویژگی‌های مدل

### Features (ورودی‌ها)
- Age (سن)
- Gender (جنسیت)
- StudyHours (ساعت مطالعه)
- AttendanceRate (نرخ حضور)
- HomeworkCompletionRate (نرخ تکمیل تکالیف)
- PreviousAverage (میانگین نمرات قبلی)
- PreviousExamScore (نمره امتحان قبلی)
- MidtermScore (نمره میان‌ترم)
- AbsenceDays (روزهای غیبت)
- SleepHours (ساعت خواب)
- ClassParticipation (مشارکت در کلاس)
- MobileUsageHours (ساعت استفاده از موبایل)
- PracticeTestCount (تعداد آزمون‌های تمرینی)

### Label (خروجی)
- **FinalScore** (نمره نهایی - بین ۰ تا ۲۰)

## 🚀 راهنمای اجرا

### پیش‌نیازها
1. نصب .NET 10 SDK
2. نصب SQL Server
3. نصب Visual Studio 2022 یا VS Code

### مراحل اجرا

#### ۱. بازیابی پکیج‌ها
```bash
cd StudentScorePrediction
dotnet restore
```

#### ۲. تنظیم Connection String
فایل `src/StudentScorePrediction.Api/appsettings.json` را باز کنید و Connection String را تنظیم کنید:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StudentScorePrediction;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

#### ۳. ایجاد Database
```bash
dotnet ef database update --project src/StudentScorePrediction.Infrastructure --startup-project src/StudentScorePrediction.Api
```

#### ۴. اجرای API
```bash
dotnet run --project src/StudentScorePrediction.Api
```

API روی آدرس `https://localhost:7001` و `http://localhost:5001` اجرا می‌شود.

#### ۵. اجرای Blazor Client (در ترمینال دیگر)
```bash
dotnet run --project src/StudentScorePrediction.Client
```

Client روی آدرس `https://localhost:7002` اجرا می‌شود.

## 📖 نحوه استفاده

### ۱. تولید Dataset
- به صفحه `/dataset` در Blazor بروید
- تعداد رکوردها را انتخاب کنید (حداقل ۱۰,۰۰۰)
- دکمه Generate Dataset را بزنید

### ۲. آموزش مدل
- به صفحه `/training` بروید
- الگوریتم مورد نظر را انتخاب کنید (FastTree, FastForest, SDCA)
- Training را شروع کنید
- Metricها را مشاهده کنید (MAE, MSE, RMSE, R²)

### ۳. فعال‌سازی مدل
- به صفحه `/models` بروید
- مدل مورد نظر را انتخاب کنید
- دکمه Activate را بزنید

### ۴. پیش‌بینی نمره
- به صفحه `/prediction` بروید
- اطلاعات دانش‌آموز را وارد کنید
- دکمه Predict Score را بزنید
- نمره پیش‌بینی شده را مشاهده کنید

### ۵. مشاهده تاریخچه
- به صفحه `/predictions` بروید
- تمام پیش‌بینی‌های انجام شده را مشاهده کنید

## 📈 الگوریتم‌های پشتیبانی شده

1. **FastTree Regression** - الگوریتم Gradient Boosting Trees
2. **FastForest Regression** - الگوریتم Random Forest
3. **SDCA Regression** - الگوریتم Stochastic Dual Coordinate Ascent

## 📊 Metricهای ارزیابی

- **MAE** (Mean Absolute Error) - میانگین خطای مطلق
- **MSE** (Mean Squared Error) - میانگین خطای مربعی
- **RMSE** (Root Mean Squared Error) - ریشه میانگین خطای مربعی
- **R²** (Coefficient of Determination) - ضریب تعیین

## 🔐 امنیت

- Input Validation برای تمام ورودی‌ها
- Global Exception Handling
- CORS Configuration
- ساختار آماده برای JWT Authentication

## 📝 توضیحات Machine Learning

### مفاهیم کلیدی

- **Feature (ویژگی)**: ورودی‌های مدل که برای پیش‌بینی استفاده می‌شوند
- **Label (برچسب)**: مقداری که مدل باید پیش‌بینی کند (نمره نهایی)
- **Regression (رگرسیون)**: نوعی از یادگیری ماشین برای پیش‌بینی مقادیر عددی
- **Training (آموزش)**: فرآیند یادگیری مدل از داده‌ها
- **Prediction (پیش‌بینی)**: استفاده از مدل آموزش دیده برای پیش‌بینی مقادیر جدید
- **Overfitting (بیش‌برازش)**: وقتی مدل بیش از حد به داده‌های آموزشی وابسته می‌شود

### Pipeline آموزش مدل

1. **Load Data**: بارگذاری داده‌ها از فایل CSV
2. **Data Cleaning**: مدیریت مقادیر Missing و Outliers
3. **Feature Engineering**: تبدیل داده‌ها به فرمت مناسب
4. **Train/Test Split**: تقسیم داده‌ها به بخش‌های آموزش و تست
5. **Model Training**: آموزش مدل با الگوریتم انتخاب شده
6. **Evaluation**: ارزیابی مدل با Metricهای مختلف
7. **Model Saving**: ذخیره مدل برای استفاده بعدی

## 🧪 Testing

برای اجرای تست‌ها:
```bash
dotnet test
```

## 📦 Docker (اختیاری)

پروژه قابلیت اجرا با Docker را دارد:
```bash
docker-compose up -d
```

## 🤝 مشارکت

برای مشارکت در پروژه:
1. Fork کنید
2. Branch جدید بسازید
3. تغییرات را commit کنید
4. Push به branch
5. Pull Request ایجاد کنید

## 📄 لایسنس

این پروژه تحت لایسنس MIT منتشر شده است.

## 👨‍💻 توسعه‌دهنده

این پروژه به عنوان یک نمونه کار حرفه‌ای برای یادگیری Machine Learning با C# طراحی شده است.
