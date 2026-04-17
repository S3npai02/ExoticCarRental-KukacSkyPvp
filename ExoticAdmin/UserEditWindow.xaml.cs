using MySql.Data.MySqlClient;
using BCrypt.Net;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ExoticAdmin
{
    public partial class UserEditWindow : Window
    {
        private User _user;
        private string connectionString = "Server=127.0.0.1;Database=exotic_rentals;Uid=root;Pwd=;";
        private byte[] _selectedImageBytes = new byte[0];

        public UserEditWindow(User user)
        {
            InitializeComponent();
            _user = user;

            if (_user.Id != 0)
            {
                txtUsername.Text = _user.Username;
                txtFullName.Text = _user.FullName;
                txtEmail.Text = _user.Email;
                txtPhone.Text = _user.PhoneNumber;
                txtLicense.Text = _user.LicenseNumber;

                int autoClearance = _user.Clearance;
                if (autoClearance == 1 && _user.IsVerified && !string.IsNullOrWhiteSpace(_user.LicenseNumber))
                {
                    autoClearance = 2;
                }
                txtClearance.Text = autoClearance.ToString();

                if (_user.DateOfBirth.HasValue) txtDob.Text = _user.DateOfBirth.Value.ToString("yyyy-MM-dd");
                if (_user.LicenseExpiryDate.HasValue) txtLicenseExpiry.Text = _user.LicenseExpiryDate.Value.ToString("yyyy-MM-dd");

                chkIsVerified.IsChecked = _user.IsVerified;
                chkIsBanned.IsChecked = _user.IsBanned;
                chkIsDriver.IsChecked = _user.IsDriver;
                txtAdminNotes.Text = _user.AdminNotes;

                if (_user.ProfilePictureData != null && _user.ProfilePictureData.Length > 0)
                {
                    _selectedImageBytes = _user.ProfilePictureData;
                    imgProfile.Source = _user.ProfileImage;
                }
            }
            else
            {
                txtClearance.Text = "1";
                chkIsDriver.IsChecked = false;
            }
        }

        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Képek|*.jpg;*.jpeg;*.png;*.bmp";
            if (ofd.ShowDialog() == true)
            {
                try
                {
                    _selectedImageBytes = File.ReadAllBytes(ofd.FileName);
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.UriSource = new Uri(ofd.FileName);
                    bmp.EndInit();
                    imgProfile.Source = bmp;
                }
                catch (Exception ex) { MessageBox.Show("Hiba a kép beolvasásakor: " + ex.Message); }
            }
        }

        private void BtnGeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            if (_user.Id == 0) { MessageBox.Show("Előbb mentsd el a felhasználót!"); return; }
            try
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$*";
                Random random = new Random();
                string newPassword = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    new MySqlCommand($"UPDATE users SET password = '{hashedPassword}' WHERE id = {_user.Id}", conn).ExecuteNonQuery();
                }
                MessageBox.Show($"Új jelszó generálva: {newPassword}");
            }
            catch (Exception ex) { MessageBox.Show("Hiba a jelszó generálásakor: " + ex.Message); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query;

                    if (_user.Id == 0)
                    {
                        query = @"INSERT INTO users (username, full_name, email, phoneNumber, clearance, license_number, date_of_birth, license_expiry_date, is_verified, password, is_banned, admin_notes, isDriver, profile_picture) 
                                  VALUES (@user, @name, @email, @phone, @clearance, @license, @dob, @expiry, @verified, '$2a$12$K7O8DqM2f8L2/VzQ4W5E.OeB8jG9h1i2k3l4m5n6o7p8q9r0s1t2u', @banned, @notes, @isdriver, @profilepic)";
                    }
                    else
                    {
                        query = @"UPDATE users SET username=@user, full_name=@name, email=@email, phoneNumber=@phone, clearance=@clearance, license_number=@license, date_of_birth=@dob, license_expiry_date=@expiry, is_verified=@verified, is_banned=@banned, admin_notes=@notes, isDriver=@isdriver, profile_picture=@profilepic WHERE id=@id";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@name", txtFullName.Text);
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@license", txtLicense.Text);

                        int finalClearance = int.TryParse(txtClearance.Text, out int c) ? c : 1;
                        bool verified = chkIsVerified.IsChecked == true;

                        if (finalClearance == 1 && verified && !string.IsNullOrWhiteSpace(txtLicense.Text))
                        {
                            finalClearance = 2;
                        }

                        cmd.Parameters.AddWithValue("@clearance", finalClearance);
                        cmd.Parameters.AddWithValue("@verified", verified ? 1 : 0);
                        cmd.Parameters.AddWithValue("@banned", chkIsBanned.IsChecked == true ? 1 : 0);
                        cmd.Parameters.AddWithValue("@notes", txtAdminNotes.Text);
                        cmd.Parameters.AddWithValue("@isdriver", chkIsDriver.IsChecked == true ? 1 : 0);
                        cmd.Parameters.AddWithValue("@profilepic", _selectedImageBytes);

                        if (DateTime.TryParse(txtDob.Text, out DateTime dob)) cmd.Parameters.AddWithValue("@dob", dob);
                        else cmd.Parameters.AddWithValue("@dob", DBNull.Value);

                        if (DateTime.TryParse(txtLicenseExpiry.Text, out DateTime expiry)) cmd.Parameters.AddWithValue("@expiry", expiry);
                        else cmd.Parameters.AddWithValue("@expiry", DBNull.Value);

                        if (_user.Id != 0) cmd.Parameters.AddWithValue("@id", _user.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Hiba a mentés során: " + ex.Message); }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}