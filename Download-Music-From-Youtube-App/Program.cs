using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

internal class Program
{
    // Đường dẫn đến "chromedriver.exe"
    private const string directory = @".\..\..\..\..\chromedriver\chromedriver.exe";

    // Link test
    // https://www.youtube.com/watch?v=OZ-nmexquIQ

    private static void Main(string[] args)
    {
        try
        {
            // Rõ tiếng việt
            Console.OutputEncoding = Encoding.UTF8;

            string enterInput = null;

            while (true)
            {
                // Menu
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("Note: If You Have Not ChromeDriver Latest New Version Or Error Application When Selecting Choose Option 2\n");
                stringBuilder.AppendLine("Please Choose Option 1 And Choose Option 2 Again\n");
                stringBuilder.AppendLine("==============================================================\n");
                stringBuilder.AppendLine("Option 1: Download New Version ChromeDriver\n");
                stringBuilder.AppendLine("Option 2: Download Music");
                Console.WriteLine(stringBuilder.ToString());

                // Chọn chức năng
                enterInput = Console.ReadLine();

                // Kiểm Tra Chọn Chức Năng
                if (!ChooseFunction(enterInput))
                {
                    continue;
                }

                Console.Clear();

                break;
            }

            // Mở trình duyệt Chrome chạy ẩn
            UndetectedChromeDriver driver = OpenChrome();

            Console.Clear();

            while (true)
            {
                try
                {
                    Console.WriteLine("Enter 'S' or 's' To Stop !!! \n");

                    Console.WriteLine("Paste Link Youtube To Convert To MP3: ");

                    // Chèn link url Youtube cần convert sang MP3
                    enterInput = Console.ReadLine();

                    if (enterInput.StartsWith("S") || enterInput.StartsWith("s"))
                    {
                        driver.Quit();
                        driver.CloseDevToolsSession();
                        return;
                    }

                    if (!CheckInput(enterInput))
                    {
                        continue;
                    }
                    else
                    {
                        Console.Clear();

                        Console.WriteLine("Processing.....");

                        if (!ProcessWeb(driver, enterInput))
                        {
                            // đóng toàn bộ chrome cũ
                            driver.Quit();
                            driver.CloseDevToolsSession();

                            // Mở trình duyệt Chrome lần nữa
                            driver = OpenChrome();
                            Console.Clear();
                            continue;
                        }

                        Console.Clear();

                        Console.WriteLine("Process Is Success !!! \n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("...Notification...\n");
                    Console.WriteLine("Something Went Wrong, Let Again !!! \n");
                    Console.WriteLine("Detail Error: {0}", ex.Message);
                    Console.WriteLine("Press To Continue...");
                    Console.ReadKey();
                    Console.Clear();
                    continue;
                }
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine("...Notification...\n");
            Console.WriteLine("Something Went Wrong, Let Again !!! \n");
            Console.WriteLine("Detail Error: {0}", ex.Message);
        }
    }

    #region Các Method Chức Năng

    #region Chọn Chức Năng
    private static bool ChooseFunction(string input)
    {
        // Chọn chức năng
        switch (input)
        {
            case "1":
                // Tải ChromeDriver mới nhất
                DownloadNewChromeDriver();
                Console.WriteLine("Downloaded ChromeDriver Latest New Version !!!");
                Console.WriteLine("Press To Continue...");
                Console.ReadKey();
                return true;
            case "2":
                return true;
        }

        Console.WriteLine("Choose Wrong...Please Choose Again !!!!");
        Console.WriteLine("Press To Continue...");
        Console.ReadKey();
        Console.Clear();
        return false;
    }
    #endregion

    #region CheckInput
    private static bool CheckInput(string urlYoutubeLink)
    {
        // Check Url Null Or Empty
        if (string.IsNullOrEmpty(urlYoutubeLink) || !urlYoutubeLink.StartsWith("https://www.youtube.com/watch?"))
        {
            Console.Clear();
            Console.WriteLine("...Notification...");
            Console.WriteLine("Wrong Format, Let Enter Again !!! \n");
            Console.WriteLine("Press To Continue...");
            Console.ReadKey();
            Console.Clear();
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion

    #region Mở ChromeDriver
    private static UndetectedChromeDriver OpenChrome()
    {
        // Chạy trình duyệt ẩn
        ChromeOptions chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless"); // Chế độ headless giúp tiết kiệm bộ nhớ
        chromeOptions.AddArgument("--disable-gpu");
        chromeOptions.AddArgument("--disable-extensions");

        var driver = UndetectedChromeDriver.Create(driverExecutablePath: directory, options: chromeOptions);

        // Mở Chorme theo link youtube đã chỉ định
        driver.GoToUrl("https://y2mate.nu/en-efXo/");

        return driver;
    }
    #endregion

    #region Xử Lý Link Để Tải Nhạc
    private static bool ProcessWeb(UndetectedChromeDriver driver, string urlYoutubeLink)
    {
        // Tìm ô "Input" theo Id để chèn url vào
        var inputField = driver.FindElement(By.Id("video"));
        // Xóa các giá trị trong "Input"
        inputField.Clear();
        // Chèn link Youtube vào ô "Input"
        inputField.SendKeys(urlYoutubeLink);

        // Tìm Button "Conver"
        var buttonConvert = driver.FindElement(By.XPath("//button[@type='submit' and text()='Convert']"));
        // Click vào "Button" để convert sang MP3
        buttonConvert.Click();
        // đếm số lần thực hiện thất bại
        int cout = 0;

        // Check button "Download" sau khi convert sang MP3
        while (true)
        {
            try
            {
                bool checkDownload = false;

                if (!checkDownload)
                {
                    // Dừng 2s để tránh server quá tải
                    Thread.Sleep(5000);
                    // Tìm nút "Download". Nếu có bị lỗi chạy xuống catch 
                    var buttonDownload = driver.FindElement(By.XPath("//button[@type='button' and text()='Download']"));
                    // Click vào nút "Download"
                    buttonDownload.Click();
                    checkDownload = true;
                }

                // Lấy tất cả các handle của cửa sổ
                var windowHandles = driver.WindowHandles;
                // kiểm tra số lượng tab chrome
                if (windowHandles.Count > 1)
                {
                    for (int i = 0; i < windowHandles.Count; i++)
                    {
                        // kiểm tra đúng url tải nhạc không
                        if (driver.SwitchTo().Window(windowHandles[i]).Title != "Y2Mate - YouTube to MP3 Converter")
                        {
                            driver.ExecuteAsyncScript("window.close()");
                        }
                    }
                }

                // Chuyển về tab gốc (tab ban đầu)
                driver.SwitchTo().Window(windowHandles[0]);

                // Tìm nút "Next" để tiếp tục xử lý link tiếp theo. Nếu có bị lỗi chạy xuống catch 
                var buttonNext = driver.FindElement(By.XPath("//button[@type='button' and text()='Next']"));
                // Click vào nút "Next"
                buttonNext.Click();

                return true;
            }
            catch
            {
                cout++;

                if (cout == 10)
                {
                    return false;
                }

                continue;
            }
        }
    }
    #endregion

    #region Tải ChromeDriver Mới Nhất
    private static void DownloadNewChromeDriver()
    {
        // Tự động tải phiên bản ChromeDriver phù hợp
        string chromeDriverNew = new DriverManager().SetUpDriver(new ChromeConfig());

        // Ghi đè file "chromedriver.exe" cũ bằng file "chromedriver.exe" mới
        File.Copy(chromeDriverNew, directory, true);
    }
    private void ChangeFileName(string oldFileName, string newFileName)
    {
        try
        {
            if (File.Exists(oldFileName))
            {
                File.Move(oldFileName, newFileName);
                Console.WriteLine("File renamed successfully from {0} to {1}", oldFileName, newFileName);
            }
            else
            {
                Console.WriteLine("File {0} does not exist.", oldFileName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error renaming file: {0}", ex.Message);
        }
    }
    #endregion

    #endregion
}