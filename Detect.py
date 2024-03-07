# coding:utf-8
import datetime
from posixpath import split
import pymssql
from bs4 import BeautifulSoup
import sys
import requests
import re
import jieba
import jieba.posseg as pseg
import string
from string import whitespace
nonalpha = string.digits + string.punctuation + string.whitespace

url = sys.argv[1]
webName = sys.argv[2]
U_ID = int(sys.argv[3])
#typeID = int(sys.argv[4])
#url = 'http://tw.class.uschoolnet.com/class/?csid=css000000015149&id=memstu&cl=1155125037-7732-6037'

res = requests.get(url)
html_page = res.content
soup = BeautifulSoup(html_page, 'html.parser')
text = soup.find_all(text=True)
    
# connection info
db = pymssql.connect( 
    host='127.0.0.1',
    user='sa',
    password = '1234',
    database = 'Hunter'
)

output = ''
blacklist = [
    '[document]',
    'noscript',
    'header',
    'html',
    'meta',
    'head', 
    'input',
    'script',
    'style'

]
count = 0
for t in text:
    if t.parent.name not in blacklist:
        text = t.text.strip()
        t.string = re.sub(r"[\n][\W]+[^\w]", "\n", text)
        output += '{} '.format(text)
        count += 1

#decides the flag
def getFirstName(messageContent):
    words = pseg.cut(messageContent)
    for word, flag in words:
        if (flag == 'nr'and len(word) > 3):
            return word

    return False

def getAllName(messageContent):
    words = pseg.cut(messageContent)
    names = []
    for word, flag in words:
        # print('%s,%s' %(word,flag))
        if flag == 'nr':#nr means people's name
            names.append(word)
    return names


def alterWordTagToX(list):
    for x in list:
        jieba.add_word(x, tag='n')

def LoadStopWord():
    # Custom stop words can be added to the list
    StopWordList = [""]

    set(StopWordList)
    alterWordTagToX(StopWordList)

phonePattern = r"[(]?([+]886[-\.\s]?[2-8]|0[2-8])[)]?[-\.\s]?\d{3,4}[-\.\s]?\d{3,4}|(\d{4}|[+]886[-\.\s]?\d{3})[-\.\s]??\d{3}[-\.\s]??\d{3}"

emailPattern = r"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+"

def func(value):
    return ''.join(value.splitlines()) # remove space


def extract_address(txt):

    pattern = r"([^\d\s][^\d\s][縣市])[\s]?(\d{1,3}|\d{1,5}|\d{1,3}[-]\d{1,3})?[\s]?(\D{2,9}?(市區|鎮區|鎮市|[鄉鎮市區]))?(\D{2,9}?[路街])[\s]?((\d+?|\D{1,2})[\s]?[段])?[\s]?((\d+?|\D{1,2})[\s]?[巷])?[\s]?(\d+?[弄])?[\s]?((\d+|\D{1,3})[\s]?[號])([\s]?(\d+?|\D{1,3}|\d+(、\d+){0,5})[\s]?[樓])?[\s]?([之][\s]?(\d+?|\D{1,3}))?"
    return re.finditer(pattern, txt, re.MULTILINE)

# creates Hunter info
cursor = db.cursor()
sql = "INSERT INTO Hunter (U_ID, Content, Time, URL, Web_name) VALUES (%s, %s, %s, %s, %s)"
cursor.execute(sql, (U_ID, output, datetime.datetime.now(), url, webName))
db.commit()

# fetches cid from new Hunter info in DB
cursor.execute("Select TOP(1) C_ID FROM Hunter WHERE U_ID ={} ORDER BY Time DESC".format(U_ID))
row = cursor.fetchone()
cid = int(row[0]) # Hunter ID

# searches for names, phones, email addresses and physical addresses
for typeID in range(1, 5):
    if (typeID == 1):
        cnt = 0
    
        LoadStopWord()
        final = []
        names = getAllName(output)

        # check if the object is already in list and length is equal to 3
        for n in names:
            if(len(n) == 3 and not n in final):
                final.append(n)

        postOutput = ""
        for x in final:
            postOutput += "{}, ".format(x)
            cnt += 1
        
        # removes last 2 characters from postOutput string ", "
        postOutput = postOutput[:-2] 
        sql = "INSERT INTO Analysis (C_ID, T_ID, Content, Count) VALUES (%s, %s, %s, %s)"
        cursor.execute(sql, (cid, typeID, postOutput, cnt))
        db.commit()
    elif(typeID == 2):
        cnt = 0

        Empty_list = []
        regex_ex = re.finditer(phonePattern, output, re.MULTILINE)
        for x in regex_ex:
            Empty_list.append(x.group(0))

        y = []
        # check if the object is already in list 
        for x in Empty_list:
            if(x not in y):
                y.append(x)

        postOutput = ""
        for x in y:
            postOutput += "{}, ".format(x)
            cnt += 1

        # removes last 2 characters from postOutput string ", "
        postOutput = postOutput[:-2]
        sql = "INSERT INTO Analysis (C_ID, T_ID, Content, Count) VALUES (%s, %s, %s, %s)"
        cursor.execute(sql, (cid, typeID, postOutput, cnt))
        db.commit()
    elif(typeID == 3):
        cnt = 0

        Empty_list = []
        regex_ex = re.finditer(emailPattern, output, re.MULTILINE)
        for x in regex_ex:
            Empty_list.append(x.group(0))

        y = []
        # check if the object is already in list 
        for x in Empty_list:
            if(x not in y):
                y.append(x)

        postOutput = ""
        for x in y:
            postOutput += "{}, ".format(x)
            cnt += 1

        # removes last 2 characters from postOutput string ", "
        postOutput = postOutput[:-2]
        sql = "INSERT INTO Analysis (C_ID, T_ID, Content, Count) VALUES (%s, %s, %s, %s)"
        cursor.execute(sql, (cid, typeID, postOutput, cnt))
        db.commit()
    elif(typeID == 4):
        cnt = 0

        #removes whitespaces and newlines
        out = func(output)
        out.translate(dict.fromkeys(map(ord, whitespace))) 
        out.replace("\\n","")
        out.replace("\\\n","")
        out.replace("\\\\n","")

        res = extract_address(out)

        '''
        for x in res: # old regex
            if(x not in y and len(x) > 8 and not reg.match(x) and ((any(char.isdigit() for char in x)) or any(i in x for i in chinese_digit)) and not re.search('[a-zA-Z]', x) and re.search('\w{2}[縣|市|區|鄉]', x)):
                x = re.sub('^[0-9]+', "", x)
                y.append(x)
        '''

        Empty_list = []
        for x in res:
            Empty_list.append(x.group(0))

        y = []
        # check if the object is already in list and length is greater or equal than 8
        for x in Empty_list:
            if(len(x) >= 8 and x not in y):
                y.append(x.replace(' ', ''))


        postOutput = ""
        for x in y:
            postOutput += "{}, ".format(x)
            cnt += 1

        # removes last 2 characters from postOutput string ", "
        postOutput = postOutput[:-2]
        sql = "INSERT INTO Analysis (C_ID, T_ID, Content, Count) VALUES (%s, %s, %s, %s)"
        cursor.execute(sql, (cid, typeID, postOutput, cnt))
        db.commit()


# print(output)
# print(count)