// کتابخانه ClosedXML برای خواندن فایل‌های Excel
using ClosedXML.Excel;

// کتابخانه اصلی ML.NET برای ساخت و آموزش مدل
using Microsoft.ML;

// کلاس‌های مربوط به داده‌های ML.NET مثل ColumnName
using Microsoft.ML.Data;

// برای کار با PersianCalendar و CultureInfo
using System.Globalization;

namespace SimpleRegression;

internal class Program
{
    // مسیر فایل Excel که اطلاعات پرونده‌ها داخل آن قرار دارد
    private const string ExcelPath = @"Data\Claims.xlsx";

    // نقطه شروع اجرای برنامه
    static void Main()
    {
        // فعال کردن نمایش صحیح حروف فارسی در Console
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            // بررسی می‌کنیم فایل Excel وجود دارد یا نه
            if (!File.Exists(ExcelPath))
            {
                // اگر فایل پیدا نشد، پیام خطا نمایش می‌دهیم
                Console.WriteLine($"Excel file not found : {ExcelPath}");

                // اجرای برنامه را متوقف می‌کنیم
                return;
            }

            // نمایش پیام به کاربر
            Console.WriteLine("Reading Excel data...");

            // تاریخ‌های RegisterDate را از فایل Excel می‌خوانیم
            var registerDates = LoadRegisterDates(ExcelPath);

            // اگر هیچ تاریخی پیدا نشد
            if (registerDates.Count == 0)
            {
                // پیام مناسب نمایش می‌دهیم
                Console.WriteLine("No valid RegisterDate was found.");

                // برنامه را متوقف می‌کنیم
                return;
            }


            // نمایش تعداد کل پرونده‌هایی که از Excel خوانده شده
            Console.WriteLine($"تعداد پرونده‌ها: {registerDates.Count}");

            // نمایش اولین تاریخ ثبت شده
            Console.WriteLine($"از: {registerDates.Min() : yyyy-MM-dd HH:mm:ss}");

            // نمایش آخرین تاریخ ثبت شده
            Console.WriteLine($"تا: {registerDates.Max() : yyyy-MM-dd HH:mm:ss}");

            // یک خط خالی برای مرتب‌تر شدن خروجی
            Console.WriteLine();

            // تبدیل تاریخ‌های خام Excel به داده قابل استفاده برای ML.NET
            var trainingData = BuildTrainingData(registerDates);

            // نمایش تعداد رکوردهای آموزشی ساخته شده
            Console.WriteLine($"تعداد داده آموزشی: {trainingData.Count}");

            // یک خط خالی
            Console.WriteLine();

            // ساخت محیط اصلی ML.NET
            // seed برای تکرارپذیر بودن عملیات تصادفی استفاده می‌شود
            var mlContext = new MLContext(seed: 42);

            // تبدیل List<ClaimInput> به IDataView که ML.NET با آن کار می‌کند
            var data = mlContext.Data.LoadFromEnumerable(trainingData);

            // ساخت Pipeline آموزش مدل
            var pipeline = mlContext.Transforms

                // چند ویژگی را داخل یک ستون به نام Features ترکیب می‌کنیم
                .Concatenate(
                    "Features",

                    // روز هفته
                    nameof(ClaimInput.DayOfWeek),

                    // ساعت
                    nameof(ClaimInput.Hour),

                    // بازه 15 دقیقه‌ای
                    nameof(ClaimInput.MinuteBucket),

                    // شماره روز نسبت به اولین روز دیتاست
                    nameof(ClaimInput.DayIndex))

                // مقادیر Features را نرمال می‌کند
                .Append(
                    mlContext.Transforms.NormalizeMinMax("Features"))

                // انتخاب الگوریتم Regression برای پیش‌بینی تعداد پرونده
                .Append(
                    mlContext.Regression.Trainers.Sdca(

                        // چیزی که می‌خواهیم پیش‌بینی کنیم
                        labelColumnName: nameof(ClaimInput.Label),

                        // ویژگی‌هایی که برای پیش‌بینی استفاده می‌شوند
                        featureColumnName: "Features"));

            // اعلام شروع آموزش
            Console.WriteLine("در حال آموزش مدل...");

            // آموزش مدل با داده‌هایی که ساختیم
            var model = pipeline.Fit(data);

            // اعلام موفق بودن آموزش
            Console.WriteLine("مدل با موفقیت آموزش داده شد.");

            // یک خط خالی
            Console.WriteLine();

            // دریافت تاریخ شمسی از کاربر
            Console.Write("تاریخ شمسی را وارد کنید (مثلاً 1405/07/20): ");
            var persianDate = Console.ReadLine();

            // دریافت ساعت شروع از کاربر
            Console.Write("ساعت شروع را وارد کنید (مثلاً 09:00): ");
            var startTimeText = Console.ReadLine();

            // دریافت ساعت پایان از کاربر
            Console.Write("ساعت پایان را وارد کنید (مثلاً 14:00): ");
            var endTimeText = Console.ReadLine();

            // بررسی می‌کنیم تاریخ و ساعت‌ها معتبر هستند یا نه
            // اگر معتبر باشند، تاریخ میلادی در startDateTime و endDateTime قرار می‌گیرد
            if (!TryParsePersianDate(
                    persianDate,
                    startTimeText,
                    endTimeText,
                    out var startDateTime,
                    out var endDateTime))
            {
                // اگر ورودی اشتباه باشد
                Console.WriteLine("تاریخ یا ساعت واردشده صحیح نیست.");

                // توقف برنامه
                return;
            }

            // ساخت موتور پیش‌بینی از روی مدل آموزش‌دیده
            var predictionEngine =
                mlContext.Model.CreatePredictionEngine<ClaimInput, ClaimPrediction>(model);

            // متغیری برای جمع کردن تمام پیش‌بینی‌های 15 دقیقه‌ای
            var totalPrediction = 0f;

            // از ساعت شروع حرکت می‌کنیم
            // تا زمانی که به ساعت پایان برسیم
            // هر بار 15 دقیقه جلو می‌رویم
            for (var current = startDateTime;
                 current < endDateTime;
                 current = current.AddMinutes(15))
            {
                // ساخت ورودی مناسب برای مدل برای زمان فعلی
                var input = CreatePredictionInput(current, registerDates.Min());

                // انجام پیش‌بینی
                var prediction = predictionEngine.Predict(input);

                // اگر مدل عدد منفی برگرداند، صفر در نظر می‌گیریم
                // چون تعداد پرونده نمی‌تواند منفی باشد
                totalPrediction += Math.Max(0, prediction.Score);
            }

            // تبدیل نتیجه نهایی به عدد صحیح
            // مثلاً 25.7 تبدیل می‌شود به 26
            var finalCount = Math.Max(
                0,
                (int)Math.Round(totalPrediction));

            // یک خط خالی
            Console.WriteLine();

            // نمایش عنوان نتیجه
            Console.WriteLine("========== نتیجه پیش‌بینی ==========");

            // نمایش تاریخ شمسی وارد شده توسط کاربر
            Console.WriteLine($"تاریخ: {persianDate}");

            // نمایش زمان شروع
            Console.WriteLine($"شروع: {startDateTime:yyyy-MM-dd HH:mm}");

            // نمایش زمان پایان
            Console.WriteLine($"پایان: {endDateTime:yyyy-MM-dd HH:mm}");

            // محاسبه و نمایش مدت زمان بازه
            Console.WriteLine($"مدت بازه: {endDateTime - startDateTime}");

            // جداکننده
            Console.WriteLine("------------------------------------");

            // نمایش تعداد نهایی پرونده پیش‌بینی شده
            Console.WriteLine($"تعداد پرونده پیش‌بینی‌شده: {finalCount}");

            // پایان بخش نتیجه
            Console.WriteLine("====================================");
        }
        catch (Exception ex)
        {
            // اگر هر خطایی در برنامه رخ دهد، وارد این بخش می‌شویم

            // یک خط خالی
            Console.WriteLine();

            // نمایش عنوان خطا
            Console.WriteLine("خطا:");

            // نمایش متن خطایی که اتفاق افتاده
            Console.WriteLine(ex.Message);
        }

        // منتظر فشردن یک کلید می‌مانیم تا Console بسته نشود
        Console.ReadKey();
    }

    // این متد تاریخ‌های RegisterDate را از فایل Excel می‌خواند
    private static List<DateTime> LoadRegisterDates(string path)
    {
        // ساخت یک لیست خالی برای نگهداری تاریخ‌ها
        var result = new List<DateTime>();

        // باز کردن فایل Excel
        // using باعث می‌شود بعد از اتمام کار فایل به‌درستی آزاد شود
        using var workbook = new XLWorkbook(path);

        // انتخاب اولین Sheet موجود در Excel
        var worksheet = workbook.Worksheets.First();

        // پیدا کردن اولین ردیفی که در Sheet استفاده شده
        var headerRow = worksheet.FirstRowUsed();

        // اگر هیچ ردیفی وجود نداشت
        if (headerRow == null)
            return result;

        // فعلاً شماره ستون RegisterDate را -1 قرار می‌دهیم
        // -1 یعنی هنوز ستون را پیدا نکرده‌ایم
        var registerDateColumn = -1;

        // بررسی تک تک سلول‌های Header
        foreach (var cell in headerRow.CellsUsed())
        {
            // بررسی می‌کنیم نام سلول RegisterDate باشد یا نه
            if (!string.Equals(
                    cell.GetString().Trim(),
                    "RegisterDate",
                    StringComparison.OrdinalIgnoreCase))
            {
                // اگر RegisterDate نبود، برو سراغ سلول بعدی
                continue;
            }

            // شماره ستون RegisterDate را ذخیره می‌کنیم
            registerDateColumn = cell.Address.ColumnNumber;

            // چون ستون را پیدا کردیم، دیگر جستجو لازم نیست
            break;
        }

        // اگر ستون RegisterDate پیدا نشده باشد
        if (registerDateColumn == -1)
            throw new Exception("ستون RegisterDate پیدا نشد.");

        // پیمایش تمام ردیف‌های Excel به جز Header
        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            // گرفتن سلول RegisterDate از ردیف فعلی
            var cell = row.Cell(registerDateColumn);

            // اگر سلول خالی بود، برو سراغ ردیف بعدی
            if (cell.IsEmpty())
                continue;

            // ابتدا تلاش می‌کنیم مقدار سلول را مستقیم به DateTime تبدیل کنیم
            if (cell.TryGetValue<DateTime>(out var dateTime))
            {
                // اگر موفق بود، تاریخ را داخل لیست قرار می‌دهیم
                result.Add(dateTime);

                // برو سراغ ردیف بعدی
                continue;
            }

            // اگر تبدیل مستقیم موفق نبود
            // مقدار سلول را به صورت متن دریافت می‌کنیم
            var text = cell.GetString().Trim();

            // تلاش می‌کنیم متن را به DateTime تبدیل کنیم
            if (DateTime.TryParse(
                    text,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateTime))
            {
                // اگر موفق شد، تاریخ را ذخیره می‌کنیم
                result.Add(dateTime);
            }
        }

        // در نهایت تاریخ‌ها را از قدیمی‌ترین به جدیدترین مرتب می‌کنیم
        return result.OrderBy(x => x).ToList();
    }

    // تبدیل تاریخ‌های خام به داده‌های آموزشی ML.NET
    private static List<ClaimInput> BuildTrainingData(
        List<DateTime> registerDates)
    {
        // پیدا کردن اولین روز دیتاست
        // .Date قسمت ساعت را حذف می‌کند
        var minDate = registerDates.Min().Date;

        // پیدا کردن آخرین روز دیتاست
        var maxDate = registerDates.Max().Date;

        // گروه‌بندی تمام پرونده‌ها در بازه‌های 15 دقیقه‌ای
        var grouped = registerDates
            .GroupBy(FloorTo15Minutes)

            // تبدیل گروه‌ها به Dictionary
            .ToDictionary(
                group => group.Key,

                // تعداد پرونده‌های هر بازه 15 دقیقه‌ای
                group => group.Count());

        // ساخت لیست نهایی داده‌های آموزشی
        var result = new List<ClaimInput>();

        // محاسبه تعداد روزهای بین اولین و آخرین روز
        var totalDays = (maxDate - minDate).Days;

        // پیمایش تمام روزها
        for (var dayIndex = 0; dayIndex <= totalDays; dayIndex++)
        {
            // ساخت تاریخ روز فعلی
            var date = minDate.AddDays(dayIndex);

            // هر روز 96 بازه 15 دقیقه‌ای دارد
            // چون 24 ساعت × 4 = 96
            for (var bucket = 0; bucket < 96; bucket++)
            {
                // ساخت زمان فعلی
                // مثلاً:
                // bucket = 0  → 00:00
                // bucket = 1  → 00:15
                // bucket = 2  → 00:30
                var current = date.AddMinutes(bucket * 15);

                // بررسی می‌کنیم آیا برای این زمان پرونده‌ای ثبت شده
                // اگر وجود نداشته باشد count برابر صفر می‌شود
                grouped.TryGetValue(current, out var count);

                // ساخت یک رکورد آموزشی
                result.Add(new ClaimInput
                {
                    // شماره روز هفته
                    DayOfWeek = (float)current.DayOfWeek,

                    // ساعت
                    Hour = current.Hour,

                    // تبدیل دقیقه به شماره بازه
                    // 00 → 0
                    // 15 → 1
                    // 30 → 2
                    // 45 → 3
                    MinuteBucket = current.Minute / 15,

                    // شماره روز در دیتاست
                    DayIndex = dayIndex,

                    // تعداد واقعی پرونده در این بازه
                    // این همان چیزی است که مدل باید یاد بگیرد
                    Label = count
                });
            }
        }

        // برگرداندن داده‌های آموزشی
        return result;
    }

    // ساخت ورودی برای پیش‌بینی یک زمان مشخص
    private static ClaimInput CreatePredictionInput(
        DateTime targetDateTime,
        DateTime minDate)
    {
        // محاسبه فاصله روز پیش‌بینی از اولین روز دیتاست
        var dayIndex = (targetDateTime.Date - minDate.Date).Days;

        // ساخت ورودی مدل
        return new ClaimInput
        {
            // روز هفته تاریخ موردنظر
            DayOfWeek = (float)targetDateTime.DayOfWeek,

            // ساعت موردنظر
            Hour = targetDateTime.Hour,

            // شماره بازه 15 دقیقه‌ای
            MinuteBucket = targetDateTime.Minute / 15,

            // شماره روز نسبت به اولین روز دیتاست
            DayIndex = dayIndex
        };
    }

    // تبدیل یک زمان به پایین‌ترین بازه 15 دقیقه‌ای
    private static DateTime FloorTo15Minutes(DateTime dateTime)
    {
        // پیدا کردن دقیقه مناسب
        // مثلاً 17 دقیقه تبدیل می‌شود به 15
        var minute = (dateTime.Minute / 15) * 15;

        // ساخت DateTime جدید با دقیقه گرد شده
        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            minute,
            0);
    }

    // تبدیل تاریخ شمسی و ساعت‌ها به DateTime قابل استفاده در C#
    private static bool TryParsePersianDate(
        string? dateText,
        string? startTimeText,
        string? endTimeText,
        out DateTime startDateTime,
        out DateTime endDateTime)
    {
        // مقدار اولیه برای زمان شروع
        startDateTime = default;

        // مقدار اولیه برای زمان پایان
        endDateTime = default;

        // بررسی خالی نبودن ورودی‌ها
        if (string.IsNullOrWhiteSpace(dateText) ||
            string.IsNullOrWhiteSpace(startTimeText) ||
            string.IsNullOrWhiteSpace(endTimeText))
        {
            // اگر هرکدام خالی بود، ورودی نامعتبر است
            return false;
        }

        // یکسان‌سازی جداکننده تاریخ
        // 1405-07-20 → 1405/07/20
        // 1405.07.20 → 1405/07/20
        var parts = dateText
            .Trim()
            .Replace("-", "/")
            .Replace(".", "/")
            .Split('/');

        // تاریخ باید دقیقاً شامل سال، ماه و روز باشد
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], out var year) ||
            !int.TryParse(parts[1], out var month) ||
            !int.TryParse(parts[2], out var day) ||
            !TimeSpan.TryParse(startTimeText.Trim(), out var startTime) ||
            !TimeSpan.TryParse(endTimeText.Trim(), out var endTime))
        {
            // اگر هرکدام نامعتبر بود، false برمی‌گردانیم
            return false;
        }

        try
        {
            // ساخت تقویم شمسی
            var calendar = new PersianCalendar();

            // تبدیل تاریخ شمسی + ساعت شروع به DateTime میلادی
            startDateTime = calendar.ToDateTime(
                year,
                month,
                day,
                startTime.Hours,
                startTime.Minutes,
                startTime.Seconds,
                0);

            // تبدیل تاریخ شمسی + ساعت پایان به DateTime میلادی
            endDateTime = calendar.ToDateTime(
                year,
                month,
                day,
                endTime.Hours,
                endTime.Minutes,
                endTime.Seconds,
                0);

            // اگر زمان پایان قبل یا مساوی شروع باشد،
            // فرض می‌کنیم پایان مربوط به روز بعد است
            //
            // مثال:
            // شروع = 23:00
            // پایان = 02:00
            //
            // در این حالت پایان یک روز جلو می‌رود
            if (endDateTime <= startDateTime)
                endDateTime = endDateTime.AddDays(1);

            // تبدیل با موفقیت انجام شده
            return true;
        }
        catch
        {
            // اگر تاریخ از نظر تقویمی نامعتبر باشد
            // مثلاً روز یا ماه اشتباه باشد
            return false;
        }
    }
}

// کلاس مربوط به داده‌ای که به مدل می‌دهیم
public class ClaimInput
{
    // روز هفته
    // مثال: Monday = 1
    public float DayOfWeek { get; set; }

    // ساعت
    // مثال: 10
    public float Hour { get; set; }

    // شماره بازه 15 دقیقه‌ای
    // 00 = 0
    // 15 = 1
    // 30 = 2
    // 45 = 3
    public float MinuteBucket { get; set; }

    // شماره روز نسبت به اولین روز دیتاست
    public float DayIndex { get; set; }

    // مقدار واقعی که مدل باید یاد بگیرد
    // در پروژه ما یعنی تعداد پرونده
    [ColumnName("Label")]
    public float Label { get; set; }
}

// کلاس مربوط به نتیجه پیش‌بینی
public class ClaimPrediction
{
    // Score نتیجه‌ای است که مدل پیش‌بینی می‌کند
    [ColumnName("Score")]
    public float Score { get; set; }
}