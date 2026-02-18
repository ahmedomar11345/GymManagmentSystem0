using System;

namespace GymManagmentBLL.Service.Implementations
{
    public static class EmailTemplates
    {
        private const string PrimaryColor = "#0d6efd";
        private const string PrimaryGradient = "linear-gradient(135deg, #0d6efd 0%, #00d2ff 100%)";
        private const string DarkColor = "#1e293b";
        private const string SecondaryColor = "#64748b";
        private const string SuccessColor = "#10b981";
        private const string DangerColor = "#ef4444";
        private const string BgColor = "#f1f5f9";

        private static string GetLayout(string title, string content, string? gymName, string? gymPhone = null, string? gymAddress = null, string? gymEmail = null, string accentColor = PrimaryColor, bool isArabic = true)
        {
            string name = (gymName ?? "IronPulse Gym").ToUpper();
            string phone = gymPhone ?? "+20 123 456 789";
            string address = gymAddress ?? "Cairo, Egypt";
            string direction = isArabic ? "rtl" : "ltr";
            string textAlign = isArabic ? "right" : "left";
            string slogan = isArabic ? "بناء القوة.. تحديد الشخصية" : "Building Strength.. Defining Character";

            return $@"
            <div dir='{direction}' style='font-family: ""Cairo"", ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa; padding: 40px 10px; text-align: {textAlign};'>
                <div style='max-width: 600px; margin: 0 auto; background-color: white; border-radius: 24px; overflow: hidden; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); border-top: 8px solid {accentColor};'>
                    
                    <!-- Header -->
                    <div style='padding: 30px; border-bottom: 2px solid #f1f5f9;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='text-align: {textAlign};'>
                                    <h1 style='margin: 0; color: {accentColor}; font-size: 24px; font-weight: 900;'>{name}</h1>
                                    <p style='margin: 5px 0 0 0; color: #64748b; font-size: 12px; font-weight: 600;'>{slogan}</p>
                                </td>
                                <td style='text-align: {(isArabic ? "left" : "right")}; vertical-align: middle;'>
                                    <div style='background-color: {accentColor}15; color: {accentColor}; padding: 6px 12px; border-radius: 10px; display: inline-block; font-weight: 800; font-size: 12px; border: 1px solid {accentColor}30;'>
                                        {title}
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Body -->
                    <div style='padding: 40px 35px; color: {DarkColor}; line-height: 1.8; font-size: 15px;'>
                        {content}
                    </div>

                    <!-- Footer -->
                    <div style='background-color: #fafafa; padding: 30px; text-align: center; border-top: 1px solid #f1f5f9;'>
                        <div style='margin-bottom: 20px;'>
                            <strong style='color: {DarkColor}; font-size: 16px;'>{name}</strong>
                        </div>
                        <div style='color: #64748b; font-size: 12px; margin-bottom: 20px;'>
                            <span style='margin: 0 8px; white-space: nowrap;'>📞 {phone}</span>
                            <span style='margin: 0 8px; white-space: nowrap;'>📍 {address}</span>
                            {(!string.IsNullOrEmpty(gymEmail) ? $"<div style='margin-top: 10px;'>✉️ {gymEmail}</div>" : "")}
                        </div>
                        <div style='border-top: 1px solid #eee; padding-top: 20px; color: #94a3b8; font-size: 11px;'>
                            &copy; {DateTime.Now.Year} {name}. {(isArabic ? "جميع الحقوق محفوظة." : "All rights reserved.")}
                        </div>
                    </div>
                </div>
            </div>";
        }

        public static string WelcomeMember(string name, string? gymName, string? gymPhone = null, string? gymAddress = null, string? gymEmail = null, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "مرحباً بك في عالم القوة";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>أهلاً بك يا بطل، {name}! 👋</p>
                    <p>نحن في غاية الحماس لانضمامك إلى مجتمعنا الرياضي المتميز في <strong>{gName}</strong>. هدفنا هو مساعدتك للوصول إلى أفضل نسخة من نفسك.</p>
                    
                    <div style='background-color: {BgColor}; border-radius: 16px; padding: 25px; margin: 30px 0; border-right: 6px solid {PrimaryColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {PrimaryColor}; font-size: 17px;'>كيف تبدأ رحلتك؟</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>✅ <strong>اختر خطتك:</strong> تصفح الباقات المتاحة واشترك في ما يناسبك.</div>
                            <div style='margin-bottom: 12px;'>✅ <strong>احجز جلسة:</strong> ابدأ أول تمرين لك مع أفضل المدربين.</div>
                            <div style='margin-bottom: 0;'>✅ <strong>تابع تقدمك:</strong> استخدم لوحة التحكم لمراقبة إنجازاتك.</div>
                        </div>
                    </div>";
            }
            else
            {
                title = "Welcome to the Club";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>Welcome Champ, {name}! 👋</p>
                    <p>We are extremely excited to have you join our premium fitness community at <strong>{gName}</strong>. Our goal is to help you become the best version of yourself.</p>
                    
                    <div style='background-color: {BgColor}; border-radius: 16px; padding: 25px; margin: 30px 0; border-left: 6px solid {PrimaryColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {PrimaryColor}; font-size: 17px;'>How to start?</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>✅ <strong>Pick a Plan:</strong> Browse available packages and subscribe.</div>
                            <div style='margin-bottom: 12px;'>✅ <strong>Book a Session:</strong> Start your first workout with top trainers.</div>
                            <div style='margin-bottom: 0;'>✅ <strong>Track Progress:</strong> Use the dashboard to monitor achievements.</div>
                        </div>
                    </div>";
            }

            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, PrimaryColor, isArabic);
        }

        public static string WelcomeTrainer(string name, string? gymName, string? gymPhone = null, string? gymAddress = null, string? gymEmail = null, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "انضمام كادر محترف";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>مرحباً كوتش {name}! 🎖️</p>
                    <p>يسعدنا جداً انضمام خبرتك الكبيرة إلى طاقم التدريب في <strong>{gName}</strong>. النظام جاهز الآن لاستقبالك وتنظيم مهامك.</p>
                    
                    <div style='background-color: #ecfdf5; border-radius: 16px; padding: 25px; margin: 30px 0; border-right: 6px solid {SuccessColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {SuccessColor}; font-size: 17px;'>أدواتك للنجاح:</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>📅 <strong>إدارة الجدول:</strong> رؤية كاملة لجلساتك اليومية والأسبوعية.</div>
                            <div style='margin-bottom: 12px;'>👥 <strong>متابعة المتدربين:</strong> تسجيل الحضور ومتابعة الحالة الصحية.</div>
                            <div style='margin-bottom: 0;'>📊 <strong>الإحصائيات:</strong> مراقبة أدائك وتفاعل المشتركين معك.</div>
                        </div>
                    </div>";
            }
            else
            {
                title = "Trainer Onboarding";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>Welcome Coach {name}! 🎖️</p>
                    <p>We are delighted to have your expertise join our training staff at <strong>{gName}</strong>. The system is ready for you to manage your tasks.</p>
                    
                    <div style='background-color: #ecfdf5; border-radius: 16px; padding: 25px; margin: 30px 0; border-left: 6px solid {SuccessColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {SuccessColor}; font-size: 17px;'>Your Tools for Success:</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>📅 <strong>Schedule Management:</strong> Full view of your sessions.</div>
                            <div style='margin-bottom: 12px;'>👥 <strong>Trainee Tracking:</strong> Record attendance and health notes.</div>
                            <div style='margin-bottom: 0;'>📊 <strong>Analytics:</strong> Monitor your performance and engagement.</div>
                        </div>
                    </div>";
            }

            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, SuccessColor, isArabic);
        }

        public static string BookingConfirmation(string memberName, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string sessionName, DateTime date, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;

            if (isArabic)
            {
                title = "تأكيد حجز الجلسة";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>تم الحجز بنجاح! 📅</p>
                    <p>أهلاً {memberName}، لقد تم تأكيد موعدك بنجاح في <strong>{gName}</strong>. نحن بانتظارك!</p>
                    
                    <div style='background-color: white; border-radius: 20px; padding: 30px; margin: 30px 0; border: 2px solid #f1f5f9;'>
                        <div style='text-align: center; border-bottom: 1px solid #f1f5f9; padding-bottom: 20px; margin-bottom: 20px;'>
                            <div style='font-size: 12px; text-transform: uppercase; color: {SecondaryColor}; font-weight: bold;'>نوع الجلسة</div>
                            <div style='font-size: 22px; font-weight: 800; color: {PrimaryColor}; margin-top: 5px;'>{sessionName}</div>
                        </div>
                        
                        <table style='width: 100%; border-collapse: collapse;' dir='rtl'>
                            <tr>
                                <td style='padding: 10px 0; color: {SecondaryColor}; font-size: 14px;'>📅 الموعد:</td>
                                <td style='padding: 10px 0; text-align: left; font-weight: 700; font-size: 14px;'>{date:dddd، dd MMMM yyyy}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; color: {SecondaryColor}; font-size: 14px;'>⏰ الحضور:</td>
                                <td style='padding: 10px 0; text-align: left; font-weight: 700; font-size: 14px;'>قبل الموعد بـ 10 دقائق</td>
                            </tr>
                        </table>
                    </div>";
            }
            else
            {
                title = "Booking Confirmation";
                content = $@"
                    <p style='font-size: 20px; font-weight: 700; margin-top: 0; color: {DarkColor};'>Booking Successful! 📅</p>
                    <p>Hello {memberName}, your session at <strong>{gName}</strong> has been confirmed. See you there!</p>
                    
                    <div style='background-color: white; border-radius: 20px; padding: 30px; margin: 30px 0; border: 2px solid #f1f5f9;'>
                        <div style='text-align: center; border-bottom: 1px solid #f1f5f9; padding-bottom: 20px; margin-bottom: 20px;'>
                            <div style='font-size: 12px; text-transform: uppercase; color: {SecondaryColor}; font-weight: bold;'>Session Type</div>
                            <div style='font-size: 22px; font-weight: 800; color: {PrimaryColor}; margin-top: 5px;'>{sessionName}</div>
                        </div>
                        
                        <table style='width: 100%; border-collapse: collapse;' dir='ltr'>
                            <tr>
                                <td style='padding: 10px 0; color: {SecondaryColor}; font-size: 14px;'>📅 Date:</td>
                                <td style='padding: 10px 0; text-align: right; font-weight: 700; font-size: 14px;'>{date:dddd, MMM dd, yyyy}</td>
                            </tr>
                            <tr>
                                <td style='padding: 10px 0; color: {SecondaryColor}; font-size: 14px;'>⏰ Arrival:</td>
                                <td style='padding: 10px 0; text-align: right; font-weight: 700; font-size: 14px;'>10 mins before start</td>
                            </tr>
                        </table>
                    </div>";
            }
            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, PrimaryColor, isArabic);
        }

        public static string SessionAssignment(string trainerName, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string sessionName, DateTime date, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "تكليف تدريبي جديد";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0;'>كوتش {trainerName}، 📋</p>
                    <p>لديك تكليف تدريبي جديد في <strong>{gName}</strong> تمت إضافته لجدولك:</p>
                    
                    <div style='background: white; border-radius: 16px; padding: 30px; margin: 30px 0; border: 2px solid #f1f5f9;'>
                        <div style='font-size: 20px; font-weight: 800; color: {PrimaryColor}; mb: 10px;'>{sessionName}</div>
                        <div style='margin-top: 15px; font-size: 14px; color: #64748b;'><strong>🗓️ اليوم:</strong> {date:dddd}</div>
                        <div style='margin-top: 10px; font-size: 14px; color: #64748b;'><strong>🕒 التاريخ:</strong> {date:dd MMMM yyyy}</div>
                    </div>";
            }
            else
            {
                title = "Training Assignment";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0;'>Coach {trainerName}, 📋</p>
                    <p>You have a new training session at <strong>{gName}</strong> assigned to your schedule:</p>
                    
                    <div style='background: white; border-radius: 16px; padding: 30px; margin: 30px 0; border: 2px solid #f1f5f9;'>
                        <div style='font-size: 20px; font-weight: 800; color: {PrimaryColor}; mb: 10px;'>{sessionName}</div>
                        <div style='margin-top: 15px; font-size: 14px; color: #64748b;'><strong>🗓️ Day:</strong> {date:dddd}</div>
                        <div style='margin-top: 10px; font-size: 14px; color: #64748b;'><strong>🕒 Date:</strong> {date:MMM dd, yyyy}</div>
                    </div>";
            }
            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, PrimaryColor, isArabic);
        }

        public static string MembershipReceipt(string memberName, string planName, decimal price, DateTime endDate, int durationDays,
            string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string gPhone = gymPhone ?? "+20 123 456 789";
            string gAddress = gymAddress ?? "Cairo, Egypt";
            string gEmail = gymEmail ?? "";
            string direction = isArabic ? "rtl" : "ltr";
            string textAlign = isArabic ? "right" : "left";
            string title = isArabic ? "إيصال دفع اشتراك" : "Membership Payment Receipt";
            
            string content = $@"
            <div dir='{direction}' style='font-family: ""Cairo"", ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f8f9fa; padding: 30px 10px; text-align: {textAlign};'>
                <div style='max-width: 650px; margin: 0 auto; background-color: white; border-radius: 20px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.08); border-top: 8px solid {PrimaryColor};'>
                    
                    <!-- Header -->
                    <div style='padding: 30px; border-bottom: 2px solid #f1f3f5;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='text-align: {textAlign};'>
                                    <h1 style='margin: 0; color: {PrimaryColor}; font-size: 26px; font-weight: 900;'>{gName.ToUpper()}</h1>
                                    <p style='margin: 5px 0 0 0; color: #64748b; font-size: 13px;'>{(isArabic ? "بناء القوة.. تحديد الشخصية" : "Building Strength.. Defining Character")}</p>
                                </td>
                                <td style='text-align: {(isArabic ? "left" : "right")};'>
                                    <h2 style='margin: 0; color: #1e293b; font-size: 22px; font-weight: 700;'>{title}</h2>
                                    <p style='margin: 5px 0 0 0; color: #94a3b8; font-size: 11px;'>#INV-{DateTime.Now:yyyyMMdd-HHmm}</p>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Info Section -->
                    <div style='padding: 30px 30px 10px 30px;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='width: 50%; vertical-align: top; padding-{(isArabic ? "left" : "right")}: 15px;'>
                                    <h6 style='margin: 0 0 10px 0; color: #94a3b8; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;'>{(isArabic ? "مقدم إلى" : "Billed To")}</h6>
                                    <p style='margin: 0; font-weight: 700; color: #1e293b; font-size: 16px;'>{memberName}</p>
                                </td>
                                <td style='width: 50%; vertical-align: top;'>
                                    <h6 style='margin: 0 0 10px 0; color: #94a3b8; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;'>{(isArabic ? "تفاصيل الاشتراك" : "Subscription Details")}</h6>
                                    <p style='margin: 0; font-weight: 600; color: #1e293b; font-size: 14px;'>{(isArabic ? "الباقة" : "Plan")}: <span style='color: {PrimaryColor};'>{planName}</span></p>
                                    <p style='margin: 5px 0 0 0; color: #64748b; font-size: 13px;'>{(isArabic ? "تاريخ الانتهاء" : "Expiry Date")}: {endDate:dd/MM/yyyy}</p>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Table Section -->
                    <div style='padding: 20px 30px;'>
                        <table style='width: 100%; border-collapse: collapse; border-radius: 12px; overflow: hidden; border: 1px solid #f1f5f9;'>
                            <thead>
                                <tr style='background-color: #f8fafc;'>
                                    <th style='padding: 15px; text-align: {textAlign}; color: #64748b; font-size: 12px; border-bottom: 2px solid #f1f5f9;'>{(isArabic ? "الوصف" : "Description")}</th>
                                    <th style='padding: 15px; text-align: center; color: #64748b; font-size: 12px; border-bottom: 2px solid #f1f5f9;'>{(isArabic ? "المدة" : "Duration")}</th>
                                    <th style='padding: 15px; text-align: center; color: #64748b; font-size: 12px; border-bottom: 2px solid #f1f5f9;'>{(isArabic ? "السعر" : "Price")}</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td style='padding: 20px 15px; border-bottom: 1px solid #f1f5f9;'>
                                        <p style='margin: 0; font-weight: 700; color: #1e293b;'>{(isArabic ? $"اشتراك نادي رياضي - {planName}" : $"Gym Membership - {planName}")}</p>
                                        <p style='margin: 5px 0 0 0; color: #94a3b8; font-size: 12px;'>{(isArabic ? "تفعيل كامل لكافة المرافق" : "Full access to all facilities")}</p>
                                    </td>
                                    <td style='padding: 20px 15px; text-align: center; border-bottom: 1px solid #f1f5f9; color: #1e293b; font-weight: 600;'>
                                        {durationDays} {(isArabic ? "يوم" : "Days")}
                                    </td>
                                    <td style='padding: 20px 15px; text-align: center; border-bottom: 1px solid #f1f5f9; font-weight: 700; color: #1e293b;'>
                                        {price:N2} {(isArabic ? "ج.م" : "EGP")}
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <!-- Total Section -->
                    <div style='padding: 10px 30px 30px 30px;'>
                        <table style='width: 100%; border-collapse: collapse;'>
                            <tr>
                                <td style='width: 60%;'></td>
                                <td style='width: 40%;'>
                                    <div style='background-color: #f8fafc; padding: 20px; border-radius: 12px;'>
                                        <table style='width: 100%; border-collapse: collapse;'>
                                            <tr>
                                                <td style='color: #64748b; font-size: 13px;'>{(isArabic ? "المبلغ الفرعي:" : "Subtotal:")}</td>
                                                <td style='text-align: {(isArabic ? "left" : "right")}; font-weight: 600;'>{price:N2}</td>
                                            </tr>
                                            <tr>
                                                <td style='color: #64748b; font-size: 13px; padding-top: 5px;'>{(isArabic ? "الضريبة (0%):" : "Tax (0%):")}</td>
                                                <td style='text-align: {(isArabic ? "left" : "right")}; padding-top: 5px;'>0.00</td>
                                            </tr>
                                            <tr style='color: {PrimaryColor};'>
                                                <td style='font-weight: 900; font-size: 18px; padding-top: 15px; border-top: 2px solid #e2e8f0;'>{(isArabic ? "الإجمالي:" : "Total:") }</td>
                                                <td style='text-align: {(isArabic ? "left" : "right")}; font-weight: 900; font-size: 18px; padding-top: 15px; border-top: 2px solid #e2e8f0;'>
                                                    {price:N2} {(isArabic ? "ج.م" : "EGP")}
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Footer -->
                    <div style='background-color: #fafafa; padding: 30px; text-align: center; border-top: 1px solid #f1f3f5;'>
                        <p style='margin: 0; color: #64748b; font-size: 14px;'>{(isArabic ? $"شكراً لاختيارك {gName}. نتمنى لك رحلة تدريبية ممتعة!" : $"Thank you for choosing {gName}. Have a great training journey!")}</p>
                        <div style='margin-top: 20px; color: #94a3b8; font-size: 12px;'>
                            <span style='margin: 0 10px;'>📞 {gPhone}</span>
                            <span style='margin: 0 10px;'>📍 {gAddress}</span>
                            {(!string.IsNullOrEmpty(gEmail) ? $"<div style='margin-top: 10px;'>✉️ {gEmail}</div>" : "")}
                        </div>
                        <p style='margin-top: 20px; font-size: 10px; color: #cbd5e1;'>&copy; {DateTime.Now.Year} {gName}. {(isArabic ? "جميع الحقوق محفوظة." : "All rights reserved.")}</p>
                    </div>

                </div>
            </div>";

            return content;
        }

        public static string ExpirationAlert(string memberName, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string planName, int daysRemaining, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "تنبيه انتهاء الاشتراك";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>صديقنا العزيز {memberName}، ⚠️</p>
                    <p>نود إخبارك أن اشتراكك الحالي في <strong>{gName}</strong> (باقة {planName}) شارف على الانتهاء:</p>
                    
                    <div style='text-align: center; margin: 35px 0;'>
                        <div style='display: inline-block; background-color: #fef2f2; border: 2px solid {DangerColor}30; border-radius: 20px; padding: 25px 50px;'>
                            <div style='font-size: 44px; font-weight: 900; color: {DangerColor};'>{daysRemaining}</div>
                            <div style='font-size: 16px; font-weight: 700; color: #64748b;'>أيام متبقية</div>
                        </div>
                    </div>";
            }
            else
            {
                title = "Membership Expiry";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>Dear {memberName}, ⚠️</p>
                    <p>Your current membership at <strong>{gName}</strong> ({planName}) is about to expire:</p>
                    
                    <div style='text-align: center; margin: 35px 0;'>
                        <div style='display: inline-block; background-color: #fef2f2; border: 2px solid {DangerColor}30; border-radius: 20px; padding: 25px 50px;'>
                            <div style='font-size: 44px; font-weight: 900; color: {DangerColor};'>{daysRemaining}</div>
                            <div style='font-size: 16px; font-weight: 700; color: #64748b;'>Days Remaining</div>
                        </div>
                    </div>";
            }
            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, DangerColor, isArabic);
        }

        public static string SessionCancelled(string userName, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string sessionName, DateTime date, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "إلغاء جلسة تمرين";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>نعتذر منك {userName}، 🛑</p>
                    <p>تم إلغاء الجلسة التالية في <strong>{gName}</strong> لأسباب فنية أو تنظيمية:</p>
                    
                    <div style='background-color: #fef2f2; border-radius: 20px; padding: 25px; margin: 30px 0; border: 2px solid #fee2e2;'>
                        <div style='font-size: 20px; font-weight: 800; color: {DangerColor}; mb: 10px;'>{sessionName}</div>
                        <div style='margin-top: 15px; font-size: 14px; color: #64748b;'><strong>🗓️ الموعد:</strong> {date:dddd، dd MMMM yyyy}</div>
                        <div style='margin-top: 10px; font-size: 13px; color: {DangerColor}; font-weight: 600;'>الحالة: تم الإلغاء</div>
                    </div>";
            }
            else
            {
                title = "Session Cancelled";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>Sorry {userName}, 🛑</p>
                    <p>The following session at <strong>{gName}</strong> has been cancelled due to technical or organizational reasons:</p>
                    
                    <div style='background-color: #fef2f2; border-radius: 20px; padding: 25px; margin: 30px 0; border: 2px solid #fee2e2;'>
                        <div style='font-size: 20px; font-weight: 800; color: {DangerColor}; mb: 10px;'>{sessionName}</div>
                        <div style='margin-top: 15px; font-size: 14px; color: #64748b;'><strong>🗓️ Date:</strong> {date:dddd, MMM dd, yyyy}</div>
                        <div style='margin-top: 10px; font-size: 13px; color: {DangerColor}; font-weight: 600;'>Status: Cancelled</div>
                    </div>";
            }
            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, DangerColor, isArabic);
        }

        public static string MemberQRCodeWithCID(string name, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string imageContentId, bool isArabic = true, string? customMessage = null)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            string message = customMessage ?? (isArabic 
                ? "فيما يلي رمز الدخول الخاص بك. يرجى إبرازه عند الدخول." 
                : "Here is your access code. Please show it at the entrance.");

            if (isArabic)
            {
                title = "هوية العضوية QR";
                content = $@"
                    <div style='text-align: center;'>
                        <p style='font-size: 18px; font-weight: 700; margin-top: 0;'>أهلاً بك في <strong>{gName}</strong> يا <span style='color: {PrimaryColor};'>{name}</span>! 👋</p>
                        <p style='font-size: 14px; color: #64748b;'>{message}</p>
                    </div>";
            }
            else
            {
                title = "Access QR Code";
                content = $@"
                    <div style='text-align: center;'>
                        <p style='font-size: 18px; font-weight: 700; margin-top: 0;'>Welcome to <strong>{gName}</strong>, <span style='color: {PrimaryColor};'>{name}</span>! 👋</p>
                        <p style='font-size: 14px; color: #64748b;'>{message}</p>
                    </div>";
            }

            content += $@"
                <div style='text-align: center; margin: 35px 0;'>
                    <div style='display: inline-block; padding: 20px; background-color: #ffffff; border: 2px dashed {PrimaryColor}40; border-radius: 24px; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.05);'>
                        <img src='cid:{imageContentId}' 
                             alt='QR Code' 
                             style='display: block; width: 180px; height: 180px; border: 0;' />
                    </div>
                </div>";

            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, PrimaryColor, isArabic);
        }

        public static string BirthdayWish(string name, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, int discountPercentage, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            string accentColor = "#f59e0b";

            if (isArabic)
            {
                title = "عيد ميلاد سعيد";
                content = $@"
                    <div style='text-align: center;'>
                        <p style='font-size: 20px; font-weight: 700; margin-top: 0;'>أهلاً {name}! 🎉</p>
                        <p>نتمنى لك عاماً مليئاً بالقوة والإنجازات في <strong>{gName}</strong>!</p>
                        
                        <div style='background-color: #fffbeb; border: 2px dashed #f59e0b40; border-radius: 20px; padding: 30px; margin: 35px 0;'>
                            <div style='font-size: 14px; color: #64748b; margin-bottom: 10px;'>هدية يوم ميلادك من أسرتنا:</div>
                            <div style='font-size: 36px; font-weight: 900; color: #d97706;'>خصم {discountPercentage}%</div>
                            <div style='font-size: 15px; font-weight: 700; margin-top: 5px;'>على تجديد اشتراكك القادم</div>
                        </div>
                    </div>";
            }
            else
            {
                title = "Happy Birthday";
                content = $@"
                    <div style='text-align: center;'>
                        <p style='font-size: 20px; font-weight: 700; margin-top: 0;'>Happy Birthday, {name}! 🎉</p>
                        <p>Wishing you a year full of strength and achievements at <strong>{gName}</strong>!</p>
                        
                        <div style='background-color: #fffbeb; border: 2px dashed #f59e0b40; border-radius: 20px; padding: 30px; margin: 35px 0;'>
                            <div style='font-size: 14px; color: #64748b; margin-bottom: 10px;'>Your birthday gift from us:</div>
                            <div style='font-size: 36px; font-weight: 900; color: #d97706;'>{discountPercentage}% OFF</div>
                            <div style='font-size: 15px; font-weight: 700; margin-top: 5px;'>On your next renewal</div>
                        </div>
                    </div>";
            }

            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, accentColor, isArabic);
        }

        public static string MembershipExpired(string memberName, string? gymName, string? gymPhone, string? gymAddress, string? gymEmail, string planName, DateTime expiredDate, bool isArabic = true)
        {
            string gName = gymName ?? "IronPulse Gym";
            string title;
            string content;
            if (isArabic)
            {
                title = "انتهاء الاشتراك";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>عزيزنا {memberName}، 🔔</p>
                    <p>نود إعلامك بأن اشتراكك في <strong>{gName}</strong> (باقة <strong>{planName}</strong>) قد انتهى بتاريخ:</p>
                    
                    <div style='text-align: center; margin: 35px 0;'>
                        <div style='display: inline-block; background-color: #fef2f2; border: 2px solid {DangerColor}30; border-radius: 20px; padding: 30px 50px;'>
                            <div style='font-size: 16px; font-weight: 700; color: #64748b; margin-bottom: 8px;'>تاريخ انتهاء الاشتراك</div>
                            <div style='font-size: 28px; font-weight: 900; color: {DangerColor};'>{expiredDate:dd / MM / yyyy}</div>
                        </div>
                    </div>

                    <div style='background-color: {BgColor}; border-radius: 16px; padding: 25px; margin: 30px 0; border-right: 6px solid {DangerColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {DangerColor}; font-size: 16px;'>⚠️ ماذا يعني هذا؟</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>🚫 <strong>الدخول:</strong> لن تتمكن من دخول النادي باستخدام بطاقتك الحالية.</div>
                            <div style='margin-bottom: 12px;'>📅 <strong>الجلسات:</strong> لن تتمكن من حجز جلسات تدريبية جديدة.</div>
                            <div style='margin-bottom: 0;'>✅ <strong>التجديد:</strong> يمكنك التجديد في أي وقت لاستعادة كامل صلاحياتك.</div>
                        </div>
                    </div>

                    <p style='text-align: center; color: #64748b; font-size: 14px; margin-top: 30px;'>نتطلع لرؤيتك مجدداً في <strong>{gName}</strong>! 💪</p>";
            }
            else
            {
                title = "Membership Expired";
                content = $@"
                    <p style='font-size: 18px; margin-top: 0; color: {DarkColor};'>Dear {memberName}, 🔔</p>
                    <p>We would like to inform you that your membership at <strong>{gName}</strong> (<strong>{planName}</strong> plan) has expired on:</p>
                    
                    <div style='text-align: center; margin: 35px 0;'>
                        <div style='display: inline-block; background-color: #fef2f2; border: 2px solid {DangerColor}30; border-radius: 20px; padding: 30px 50px;'>
                            <div style='font-size: 16px; font-weight: 700; color: #64748b; margin-bottom: 8px;'>Membership Expired On</div>
                            <div style='font-size: 28px; font-weight: 900; color: {DangerColor};'>{expiredDate:dd / MM / yyyy}</div>
                        </div>
                    </div>

                    <div style='background-color: {BgColor}; border-radius: 16px; padding: 25px; margin: 30px 0; border-left: 6px solid {DangerColor}; font-size: 14px;'>
                        <h3 style='margin-top: 0; color: {DangerColor}; font-size: 16px;'>⚠️ What does this mean?</h3>
                        <div style='margin-top: 15px;'>
                            <div style='margin-bottom: 12px;'>🚫 <strong>Access:</strong> You will no longer be able to enter the gym with your current card.</div>
                            <div style='margin-bottom: 12px;'>📅 <strong>Sessions:</strong> You will not be able to book new training sessions.</div>
                            <div style='margin-bottom: 0;'>✅ <strong>Renewal:</strong> You can renew anytime to restore full access.</div>
                        </div>
                    </div>

                    <p style='text-align: center; color: #64748b; font-size: 14px; margin-top: 30px;'>We look forward to seeing you again at <strong>{gName}</strong>! 💪</p>";
            }
            return GetLayout(title, content, gymName, gymPhone, gymAddress, gymEmail, DangerColor, isArabic);
        }
    }
}
