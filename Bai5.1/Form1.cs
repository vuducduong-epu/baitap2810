using System;
using System.Drawing;
using MySql.Data.MySqlClient; // Thư viện kết nối MySQL
using System.Windows.Forms;

namespace LoginFormUX
{
    public partial class Form1 : Form
    {
        // Khai báo các Controls giao diện bằng code
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnExit;
        private ErrorProvider errorProvider1;

        // Chuỗi kết nối MySQL (XAMPP / Navicat)
        private string connectionString = "Server=localhost;Port=3306;Database=LoginFormDB;Uid=root;Pwd=;";

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();

            // Tự động tạo bảng Users và tài khoản mẫu trong Navicat nếu chưa có
            CreateTableAndDefaultData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hàm sự kiện bắt buộc để khớp với Designer
        }

        // Thiết kế giao diện hoàn toàn bằng code C#
        private void InitializeCustomComponents()
        {
            // 1. Cấu hình Form chính
            this.Text = "Đăng Nhập & MySQL (Navicat)";
            this.Size = new Size(440, 360);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 2. Khởi tạo ErrorProvider
            this.errorProvider1 = new ErrorProvider();

            // 3. Tiêu đề Form
            lblTitle = new Label();
            lblTitle.Text = "ĐĂNG NHẬP (NAVICAT / MYSQL)";
            lblTitle.Font = new Font("Arial", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(70, 20);
            this.Controls.Add(lblTitle);

            // 4. Tên đăng nhập
            lblUsername = new Label();
            lblUsername.Text = "Tên đăng nhập:";
            lblUsername.Location = new Point(40, 75);
            lblUsername.AutoSize = true;
            this.Controls.Add(lblUsername);

            txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new Point(150, 72);
            txtUsername.Size = new Size(220, 22);
            this.Controls.Add(txtUsername);

            // 5. Mật khẩu
            lblPassword = new Label();
            lblPassword.Text = "Mật khẩu:";
            lblPassword.Location = new Point(40, 120);
            lblPassword.AutoSize = true;
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new Point(150, 117);
            txtPassword.Size = new Size(220, 22);
            txtPassword.UseSystemPasswordChar = true; // Mặc định ẩn mật khẩu (*)
            this.Controls.Add(txtPassword);

            // 6. CheckBox Hiện mật khẩu
            chkShowPassword = new CheckBox();
            chkShowPassword.Name = "chkShowPassword";
            chkShowPassword.Text = "Hiện mật khẩu";
            chkShowPassword.Location = new Point(150, 155);
            chkShowPassword.AutoSize = true;
            chkShowPassword.CheckedChanged += new EventHandler(chkShowPassword_CheckedChanged);
            this.Controls.Add(chkShowPassword);

            // 7. Nút Đăng nhập
            btnLogin = new Button();
            btnLogin.Name = "btnLogin";
            btnLogin.Text = "Đăng nhập";
            btnLogin.Location = new Point(40, 205);
            btnLogin.Size = new Size(100, 35);
            btnLogin.BackColor = Color.FromArgb(0, 122, 204);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Click += new EventHandler(btnLogin_Click);
            this.Controls.Add(btnLogin);

            // 8. Nút Đăng ký tài khoản mới
            btnRegister = new Button();
            btnRegister.Name = "btnRegister";
            btnRegister.Text = "Đăng ký mới";
            btnRegister.Location = new Point(155, 205);
            btnRegister.Size = new Size(100, 35);
            btnRegister.BackColor = Color.FromArgb(40, 167, 69);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Click += new EventHandler(btnRegister_Click);
            this.Controls.Add(btnRegister);

            // 9. Nút Thoát
            btnExit = new Button();
            btnExit.Name = "btnExit";
            btnExit.Text = "Thoát";
            btnExit.Location = new Point(270, 205);
            btnExit.Size = new Size(100, 35);
            btnExit.BackColor = Color.FromArgb(204, 51, 0);
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Click += new EventHandler(btnExit_Click);
            this.Controls.Add(btnExit);

            // Phím tắt UX: Enter = Đăng nhập, Esc = Thoát
            this.AcceptButton = btnLogin;
            this.CancelButton = btnExit;
        }

        // Tự động tạo bảng Users và tài khoản admin/123456 nếu chưa có
        private void CreateTableAndDefaultData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string createTable = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Username VARCHAR(50) PRIMARY KEY,
                            Password VARCHAR(50) NOT NULL
                        );";
                    using (MySqlCommand cmd = new MySqlCommand(createTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Thêm tài khoản mẫu admin / 123456 nếu bảng chưa có
                    string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
                    using (MySqlCommand cmd = new MySqlCommand(checkAdmin, conn))
                    {
                        long count = (long)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            string insertAdmin = "INSERT INTO Users (Username, Password) VALUES ('admin', '123456')";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertAdmin, conn))
                            {
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối MySQL (Hãy kiểm tra XAMPP đã bật chưa): " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Ẩn/Hiện mật khẩu
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // Đăng ký tài khoản mới (INSERT vào MySQL)
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu để đăng ký!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Users (Username, Password) VALUES (@user, @pass)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@pass", pass);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Đăng ký thành công! Dữ liệu đã được lưu vào MySQL.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MySqlException)
            {
                MessageBox.Show("Tên đăng nhập này đã tồn tại trong cơ sở dữ liệu!", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Đăng nhập (SELECT kiểm tra từ MySQL)
        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider1.SetError(txtUsername, "Tên đăng nhập không được để trống!");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(txtUsername, "");
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider1.SetError(txtPassword, "Mật khẩu không được để trống!");
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(txtPassword, "");
            }

            if (isValid)
            {
                string inputUser = txtUsername.Text.Trim();
                string inputPass = txtPassword.Text;
                bool loginSuccess = false;

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT COUNT(*) FROM Users WHERE Username = @user AND Password = @pass";
                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@user", inputUser);
                            cmd.Parameters.AddWithValue("@pass", inputPass);

                            long count = (long)cmd.ExecuteScalar();
                            if (count > 0)
                            {
                                loginSuccess = true;
                            }
                        }
                    }

                    if (loginSuccess)
                    {
                        MessageBox.Show("Đăng nhập thành công từ MySQL!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý Thoát
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
