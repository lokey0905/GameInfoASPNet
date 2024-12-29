# 遊戲資訊網
## 摘要
一個使用ASP.NET MVC製作的一個小網站，整合遊戲新聞資訊、Steam與Epic免費遊戲等等資訊

## 介紹
- 訪客功能
1. 檢視遊戲新聞資訊 ([4Gamers](https://www.4gamers.com.tw/rss/latest-news))
2. Steam 遊戲限時免費資訊 ([freesteam](https://freesteam.games/category/steam/feed))
3. Epic 遊戲限時免費資訊 ([freesteam](https://freesteam.games/category/epic-games/feed))
4. Ubisoft 遊戲限時免費資訊 ([freesteam](https://freesteam.games/category/ubisoft/feed))
5. EA 遊戲限時免費資訊 ([freesteam](https://freesteam.games/category/electronic-arts/feed))
6. GOG遊戲限時免費資訊 ([freesteam](https://freesteam.games/category/gog/feed))
7. 註冊
8. 登入
   
- 使用者功能
1. 同訪客功能
2. 驗證若18歲以上可檢閱限制級遊戲資訊
3. 登出
   
- 管理者功能
1. 同使用者功能
2. 無須驗證年齡

## 資料庫架構
使用者資料庫，管理者可以自由新增刪除

<table>
	<tr>
	    <td>表單名稱</td>
	    <td>資料欄位</td>
	    <td>資料型態</td>  
      <td>資料型態</td>  
	</tr >
	<tr>
	    <td rowspan="7">tMember</td>
	    <td>fId</td>
	    <td>int</td>
      <td>使用者ID</td>
	</tr>
  <tr>
	    <td>fUserId</td>
	    <td>NVARCHAR (50)</td>
      <td>使用者帳號</td>
	</tr>
  <tr>
	    <td>fPwd</td>
	    <td>NVARCHAR (50)</td>
      <td>使用者密碼</td>
	</tr>
  <tr>
	    <td>fName</td>
	    <td>NVARCHAR (50)</td>
      <td>使用者名稱</td>
	</tr>
  <tr>
	    <td>fEmail</td>
	    <td>NVARCHAR (50)</td>
      <td>使用者信箱</td>
	</tr>
  <tr>
	    <td>fBirth</td>
	    <td>DATETIME</td>
      <td>使用者生日，用於驗證年齡</td>
	</tr>
  <tr>
	    <td>fIsAdmin</td>
	    <td>BIT</td>
      <td>是否為管理員</td>
	</tr>
</table>
