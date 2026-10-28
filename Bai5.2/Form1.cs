using System;
using System.Collections.Generic;
using System.Drawing;
using MySql.Data.MySqlClient; // Thư viện kết nối MySQL (yêu cầu đã cài MySql.Data ở Bước 1)
using System.Windows.Forms;

namespace StudentRegistrationSQL
{
    public partial class Form1 : Form
    {
        // Khai báo các Controls giao diện bằng code
        private Label lblTitle;
        private Label lblFullName;
        private TextBox txtFullName;

        private Label lblPhone;
        private MaskedTextBox mtxtPhone; // Yêu cầu: Mask SĐT (000) 000-0000[cite: 7]

        private Label lblBirthDate;
        private DateTimePicker dtpBirthDate; // Yêu cầu: Format Short (dd/MM/yyyy)[cite: 7]

        private Label lblGender;
        private RadioButton radMale;
        private RadioButton radFemale;

        private Label lblCourse;
        private ComboBox cboCourse; // Yêu cầu: Nạp danh sách Khóa học (DisplayMember, ValueMember)[cite: 7]

        private CheckBox chkAgree;
        private Button btnRegister; // Yêu cầu: Nút Đăng ký tổng hợp thông tin[cite: 7]
        private Button btnExit;
        private ErrorProvider errorProvider1;

        // Chuỗi kết nối MySQL / Navicat (XAMPP mặc định)
        private string connectionString = "Server=localhost;Port=3306;Database=StudentDB;Uid=root;Pwd=;";

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();

            // Tự động tạo Database và bảng Students nếu chưa có
            CreateDatabaseAndTableIfNotExists();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hàm sự kiện bắt buộc để khớp với Designer
        }

        // Thiết kế giao diện hoàn toàn bằng code C#
        private void InitializeCustomComponents()
        {
            // 1. Cấu hình Form chính
            this.Text = "Đăng Ký Học Viên & Kết Nối Navicat";
            this.Size = new Size(460, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 2. Khởi tạo ErrorProvider
            this.errorProvider1 = new ErrorProvider();

            // 3. Tiêu đề Form
            lblTitle = new Label();
            lblTitle.Text = "ĐĂNG KÝ HỌC VIÊN (NAVICAT / MYSQL)";
            lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(50, 20);
            this.Controls.Add(lblTitle);

            // 4. Họ và tên học viên
            lblFullName = new Label();
            lblFullName.Text = "Họ và tên:";
            lblFullName.Location = new Point(30, 75);
            lblFullName.AutoSize = true;
            this.Controls.Add(lblFullName);

            txtFullName = new TextBox();
            txtFullName.Name = "txtFullName";
            txtFullName.Location = new Point(150, 72);
            txtFullName.Size = new Size(260, 22);
            this.Controls.Add(txtFullName);

            // 5. Số điện thoại (MaskedTextBox)
            lblPhone = new Label();
            lblPhone.Text = "Số điện thoại:";
            lblPhone.Location = new Point(30, 115);
            lblPhone.AutoSize = true;
            this.Controls.Add(lblPhone);

            mtxtPhone = new MaskedTextBox();
            mtxtPhone.Name = "mtxtPhone";
            mtxtPhone.Location = new Point(150, 112);
            mtxtPhone.Size = new Size(260, 22);
            mtxtPhone.Mask = "(000) 000-0000"; // Yêu cầu chuẩn: (000) 000-0000[cite: 7]
            this.Controls.Add(mtxtPhone);

            // 6. Ngày sinh (DateTimePicker)
            lblBirthDate = new Label();
            lblBirthDate.Text = "Ngày sinh:";
            lblBirthDate.Location = new Point(30, 155);
            lblBirthDate.AutoSize = true;
            this.Controls.Add(lblBirthDate);

            dtpBirthDate = new DateTimePicker();
            dtpBirthDate.Name = "dtpBirthDate";
            dtpBirthDate.Location = new Point(150, 152);
            dtpBirthDate.Size = new Size(260, 22);
            dtpBirthDate.Format = DateTimePickerFormat.Custom;
            dtpBirthDate.CustomFormat = "dd/MM/yyyy"; // Yêu cầu chuẩn: dd/MM/yyyy[cite: 7]
            this.Controls.Add(dtpBirthDate);

            // 7. Giới tính (RadioButton)
            lblGender = new Label();
            lblGender.Text = "Giới tính:";
            lblGender.Location = new Point(30, 195);
            lblGender.AutoSize = true;
            this.Controls.Add(lblGender);

            radMale = new RadioButton();
            radMale.Text = "Nam";
            radMale.Location = new Point(150, 194);
            radMale.AutoSize = true;
            radMale.Checked = true;
            this.Controls.Add(radMale);

            radFemale = new RadioButton();
            radFemale.Text = "Nữ";
            radFemale.Location = new Point(230, 194);
            radFemale.AutoSize = true;
            this.Controls.Add(radFemale);

            // 8. Khóa học (ComboBox với Binding Data)
            lblCourse = new Label();
            lblCourse.Text = "Chọn khóa học:";
            lblCourse.Location = new Point(30, 235);
            lblCourse.AutoSize = true;
            this.Controls.Add(lblCourse);

            cboCourse = new ComboBox();
            cboCourse.Name = "cboCourse";
            cboCourse.Location = new Point(150, 232);
            cboCourse.Size = new Size(260, 24);
            cboCourse.DropDownStyle = ComboBoxStyle.DropDownList;

            // Nạp danh sách Khóa học thông qua DataSource (DisplayMember, ValueMember)[cite: 7]
            LoadCourseData();
            this.Controls.Add(cboCourse);

            // 9. CheckBox xác nhận thông tin
            chkAgree = new CheckBox();
            chkAgree.Text = "Tôi cam kết thông tin đăng ký là chính xác";
            chkAgree.Location = new Point(150, 275);
            chkAgree.AutoSize = true;
            this.Controls.Add(chkAgree);

            // 10. Nút Đăng ký
            btnRegister = new Button();
            btnRegister.Name = "btnRegister";
            btnRegister.Text = "Đăng ký";
            btnRegister.Location = new Point(180, 320);
            btnRegister.Size = new Size(100, 35);
            btnRegister.BackColor = Color.FromArgb(0, 122, 204);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += new EventHandler(btnRegister_Click);
            this.Controls.Add(btnRegister);

            // 11. Nút Thoát
            btnExit = new Button();
            btnExit.Name = "btnExit";
            btnExit.Text = "Thoát";
            btnExit.Location = new Point(290, 320);
            btnExit.Size = new Size(100, 35);
            btnExit.BackColor = Color.FromArgb(204, 51, 0);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Click += new EventHandler(btnExit_Click);
            this.Controls.Add(btnExit);

            // Phím tắt UX: Enter = Đăng ký, Esc = Thoát
            this.AcceptButton = btnRegister;
            this.CancelButton = btnExit;
        }

        // Nạp danh sách khóa học sử dụng DisplayMember và ValueMember[cite: 7]
        private void LoadCourseData()
        {
            List<CourseItem> courseList = new List<CourseItem>()
            {
                new CourseItem { CourseId = "C01", CourseName = "Lập trình C# WinForms" },
                new CourseItem { CourseId = "C02", CourseName = "Lập trình Web ASP.NET" },
                new CourseItem { CourseId = "C03", CourseName = "Cơ sở dữ liệu SQL & MySQL" },
                new CourseItem { CourseId = "C04", CourseName = "Lập trình hướng đối tượng" }
            };

            cboCourse.DataSource = courseList;
            cboCourse.DisplayMember = "CourseName"; // Hiển thị tên khóa học[cite: 7]
            cboCourse.ValueMember = "CourseId";     // Giá trị mã khóa học ngầm[cite: 7]
        }

        // Tự động tạo Database StudentDB và bảng Students trên MySQL/Navicat
        private void CreateDatabaseAndTableIfNotExists()
        {
            try
            {
                string serverConn = "Server=localhost;Port=3306;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(serverConn))
                {
                    conn.Open();
                    string createDb = "CREATE DATABASE IF NOT EXISTS StudentDB;";
                    using (MySqlCommand cmd = new MySqlCommand(createDb, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string createTable = @"
                        CREATE TABLE IF NOT EXISTS Students (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            FullName VARCHAR(100) NOT NULL,
                            Phone VARCHAR(20) NOT NULL,
                            BirthDate VARCHAR(20) NOT NULL,
                            Gender VARCHAR(10) NOT NULL,
                            CourseName VARCHAR(100) NOT NULL
                        );";
                    using (MySqlCommand cmd = new MySqlCommand(createTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối MySQL (Hãy chắc chắn XAMPP đã Start MySQL): " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện Đăng ký học viên
        private void btnRegister_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                errorProvider1.SetError(txtFullName, "Họ tên không được để trống!");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(txtFullName, "");
            }

            if (!mtxtPhone.MaskCompleted) // Kiểm tra SĐT đã nhập đủ theo mask[cite: 7]
            {
                errorProvider1.SetError(mtxtPhone, "Vui lòng nhập đủ số điện thoại theo định dạng!");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(mtxtPhone, "");
            }

            if (!chkAgree.Checked)
            {
                MessageBox.Show("Bạn phải tích chọn cam kết thông tin chính xác!", "Nhắc nhở", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isValid)
            {
                string fullName = txtFullName.Text.Trim();
                string phone = mtxtPhone.Text;
                string birthDate = dtpBirthDate.Text;
                string gender = radMale.Checked ? "Nam" : "Nữ";
                string courseName = cboCourse.Text; //[cite: 7]
                string courseId = cboCourse.SelectedValue.ToString(); //[cite: 7]

                try
                {
                    // Lưu dữ liệu học viên vào cơ sở dữ liệu MySQL thông qua lệnh INSERT
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "INSERT INTO Students (FullName, Phone, BirthDate, Gender, CourseName) VALUES (@name, @phone, @dob, @gender, @course)";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", fullName);
                            cmd.Parameters.AddWithValue("@phone", phone);
                            cmd.Parameters.AddWithValue("@dob", birthDate);
                            cmd.Parameters.AddWithValue("@gender", gender);
                            cmd.Parameters.AddWithValue("@course", courseName);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Tổng hợp thông tin hiển thị lên MessageBox theo đúng yêu cầu[cite: 7]
                    string message = $"ĐĂNG KÝ VÀ LƯU VÀO NAVICAT THÀNH CÔNG!\n\n" +
                                     $"- Họ và tên: {fullName}\n" +
                                     $"- Số điện thoại: {phone}\n" +
                                     $"- Ngày sinh: {birthDate}\n" +
                                     $"- Giới tính: {gender}\n" +
                                     $"- Khóa học: {courseName} (Mã: {courseId})";

                    MessageBox.Show(message, "Thông Báo Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }

    // Class cấu trúc dữ liệu cho ComboBox
    public class CourseItem
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }
    }
}
