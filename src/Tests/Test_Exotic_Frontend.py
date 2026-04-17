import unittest
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time

class ExoticRentalsTests(unittest.TestCase):
    
    @classmethod
    def setUpClass(cls):
        print("Böngésző elindítása...")
        cls.driver = webdriver.Chrome()
        cls.driver.maximize_window()
        cls.BASE_URL = "http://localhost:3000"
        
    @classmethod
    def tearDownClass(cls):
        print("Tesztek befejeződtek. Böngésző bezárása...")
        time.sleep(2)
        cls.driver.quit()

    def test_1_successful_login(self):
        """1. Teszt: Bejelentkezési folyamat a Navbaron keresztül"""
        driver = self.driver
        driver.get(self.BASE_URL)
        
        wait = WebDriverWait(driver, 10)
        time.sleep(2)
        
        login_dropdown_btn = wait.until(
            EC.element_to_be_clickable((By.XPATH, "//button[contains(text(), 'Bejelentkezés')]"))
        )
        login_dropdown_btn.click()
        
        time.sleep(1)
        
        email_input = wait.until(EC.visibility_of_element_located((By.XPATH, "//input[@type='email']")))
        password_input = driver.find_element(By.XPATH, "//input[@type='password']")
        
        
        email_input.send_keys("gamelife9222@gmail.com") 
        time.sleep(0.5)
        password_input.send_keys("admin1") 
        time.sleep(1)
        
        submit_btn = driver.find_element(By.XPATH, "//button[@type='submit' and contains(text(), 'Bejelentkezés')]")
        submit_btn.click()
        
        print("Várakozás a bejelentkezésre...")
        time.sleep(2) 
        
        profile_btn = wait.until(
            EC.visibility_of_element_located((By.XPATH, "//button[contains(text(), 'Profil')]"))
        )
        self.assertTrue(profile_btn.is_displayed(), "A bejelentkezés nem sikerült, a 'Profil' gomb nem jelent meg.")
        print("1. Teszt SIKERES: Bejelentkezve maradunk a következő teszthez!")


    def test_2_navigation_to_gallery(self):
        """2. Teszt: Egyszerű navigáció a Galéria oldalra az oldalsávból (Sidebar)"""
        driver = self.driver
        wait = WebDriverWait(driver, 10)
        
        hamburger_icon = wait.until(
            EC.element_to_be_clickable((By.CSS_SELECTOR, ".hamburger-icon"))
        )
        hamburger_icon.click()
        time.sleep(1)
        
        gallery_link = wait.until(
            EC.element_to_be_clickable((By.XPATH, "//a[contains(@class, 'sidebar-item') and contains(text(), 'Galéria')]"))
        )
        gallery_link.click()
        
        print("Navigálás a Galéria oldalra...")
        time.sleep(2) 
     
        current_url = driver.current_url
        self.assertIn("/Gallery", current_url, "Az URL nem változott meg a /Gallery végződésre!")
        
        gallery_title = wait.until(
            EC.visibility_of_element_located((By.XPATH, "//h1[contains(@class, 'gallery-hero-title')]"))
        )
        self.assertEqual(gallery_title.text, "Galéria", "A Galéria oldal főcíme nem töltött be helyesen!")
        
        print("2. Teszt SIKERES: A navigáció és a Routing hibátlanul működik!")

if __name__ == "__main__":
    unittest.main()