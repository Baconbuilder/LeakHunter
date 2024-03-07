# coding:utf-8
from ast import Add
import datetime
from posixpath import split
import pymssql
import sys
import os
import jieba.posseg as pseg
import pandas as pd
import xlsxwriter
import tkinter as tk
from tkinter import filedialog
from tkinter.filedialog import asksaveasfile

root = tk.Tk()
root.attributes("-topmost", True)
root.withdraw()
C_ID = int(sys.argv[1])
#U_ID = 2
#A_ID = 1178
# C_ID = 2090
#C_ID = 2052

document_path = os.path.expanduser('~\Downloads')
document_path += "\\"
# print(document_path)
# connection info
db = pymssql.connect( 
    host='127.0.0.1',
    user='sa',
    password = '1234',
    database = 'Hunter'
)

# fetches cid from new Hunter info in DB
cursor = db.cursor()
cursor.execute("Select CONVERT(nvarchar(4000), Content) FROM Analysis WHERE C_ID = {} AND T_ID = 1".format(C_ID))
# cursor.execute("Select Content FROM Analysis WHERE A_ID = {}".format(A_ID))
row = cursor.fetchone()
t1 = (row[0]) # Hunter ID
nameList = [x.strip() for x in t1.split(',')]

cursor.execute("Select CONVERT(nvarchar(4000), Content) FROM Analysis WHERE C_ID = {} AND T_ID = 2".format(C_ID))
# cursor.execute("Select Content FROM Analysis WHERE A_ID = {}".format(A_ID))
row = cursor.fetchone()
t2 = (row[0]) # Hunter ID
phoneList = [x.strip() for x in t2.split(',')]

cursor.execute("Select CONVERT(nvarchar(4000), Content) FROM Analysis WHERE C_ID = {} AND T_ID = 3".format(C_ID))
# cursor.execute("Select Content FROM Analysis WHERE A_ID = {}".format(A_ID))
row = cursor.fetchone()
t3 = (row[0]) # Hunter ID
mailList = [x.strip() for x in t3.split(',')]

cursor.execute("Select CONVERT(nvarchar(4000), Content) FROM Analysis WHERE C_ID = {} AND T_ID = 4".format(C_ID))
# cursor.execute("Select Content FROM Analysis WHERE A_ID = {}".format(A_ID))
row = cursor.fetchone()
t4 = (row[0]) # Hunter ID
addressList = [x.strip() for x in t4.split(',')]

cursor.execute("Select CONVERT(nvarchar(4000), Content) FROM Analysis WHERE C_ID = {} AND T_ID = 4".format(C_ID))
# cursor.execute("Select Content FROM Analysis WHERE A_ID = {}".format(A_ID))
row = cursor.fetchone()
t4 = (row[0]) # Hunter ID
addressList = [x.strip() for x in t4.split(',')]

cursor.execute("Select Web_name FROM Hunter WHERE C_ID = {}".format(C_ID))
row = cursor.fetchone()
webName = (row[0]) # Hunter ID

cursor.execute("Select URL FROM Hunter WHERE C_ID = {}".format(C_ID))
row = cursor.fetchone()
url = (row[0]) # Hunter ID

cursor.execute("Select Time FROM Hunter WHERE C_ID = {}".format(C_ID))
row = cursor.fetchone()
btime = (row[0]) # Hunter ID
atime = str(btime)

exportFile = filedialog.asksaveasfile(mode='a', title="Save the file", filetypes=[("Excel Workbook", ".xlsx")], initialfile = u"掃描結果", defaultextension=".xlsx")
a = str(exportFile.name)

workbook = xlsxwriter.Workbook(a)
worksheet = workbook.add_worksheet()

List = [nameList, phoneList, mailList, addressList]     

row_num = 0

worksheet.set_column('B:B', 15)
worksheet.set_column('C:C', 30)
worksheet.set_column('D:D', 35)

style = workbook.add_format({'bold': True})
style.set_font_color('#17202A')
style.set_bg_color('#E5E7E9')

description = workbook.add_format({'bold': True})

worksheet.write('A1', "自訂網名 : " + webName, description)
worksheet.write('A2', "掃描網址 : " + url, description)
worksheet.write('A3', "掃描時間 : " + atime, description)

worksheet.write('A5', '人名', style)
worksheet.write('B5', '電話', style)
worksheet.write('C5', '電子信箱', style)
worksheet.write('D5', '地址', style)

for col_num, data in enumerate(List):
    worksheet.write_column(row_num+5, col_num, data)

workbook.close()