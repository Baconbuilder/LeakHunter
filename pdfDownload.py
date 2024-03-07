# coding:utf-8
from lib2to3.pgen2 import driver
from selenium import webdriver
import img2pdf
from PIL import Image
import time
from selenium.webdriver.chrome.options import Options
import sys
import requests
import tkinter as tk
from tkinter import filedialog
import os
import pandas as pd
from tkinter.filedialog import asksaveasfile

root = tk.Tk()
root.attributes("-topmost", True)
root.withdraw()

chrome_options = Options()
#chrome_options.add_argument("--disable-extensions")
#chrome_options.add_argument("--disable-gpu")
#chrome_options.add_argument("--no-sandbox") # linux only
chrome_options.add_argument("--headless")
driver = webdriver.Chrome(r"chromedriver.exe", options=chrome_options)
# url = "https://localhost:7257/DataProcessing/UserRecords_class/64"
url = str(sys.argv[1])
driver.get(url)
# driver.execute_script("document.body.style.zoom='50%'")
driver.set_window_size(1920, 1080, driver.window_handles[0])
driver.maximize_window()
#time.sleep(5)
driver.save_screenshot("image.png")
driver.quit()
image1 = Image.open("image.png")
# im1 = image1.convert("RGB")
# pdfpath = r'C:\Users\mis\Desktop\WebTest\Output\blah.pdf'
# im1.save(pdfpath)
im1 = image1.crop((400, 170, 1500, 835)) # left, top, right, bottom

exportFile = filedialog.asksaveasfile(mode='a', title="Save the file", filetypes=[("PDF", ".pdf")], initialfile = u"掃描結果", defaultextension=".pdf")
b = str(exportFile.name)

pdf_path = r'Temp\temp.png'

im1.save(pdf_path)
# final.save(r'C:\Users\mis\Desktop\WebTest\Output\blah.pdf')

image = Image.open(pdf_path)

pdf_bytes = img2pdf.convert(image.filename)

file = open(b, "wb")

file.write(pdf_bytes)

im1.close()

file.close()
