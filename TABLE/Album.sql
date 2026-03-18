-- 相簿群組
CREATE TABLE Album_Group (
    Group_Id        INT PRIMARY KEY IDENTITY(1,1), -- 群組代號
    Parent_Id       INT NULL,                      -- 父群組 ID (NULL 代表第一層)
    Icon            NVARCHAR(50) NULL,             -- PrimeIcons 代碼 (如 pi-home)
    Group_Name      NVARCHAR(255) NOT NULL,        -- 群組名
    Sort_Order      INT DEFAULT 0,                 -- 排序用
    Create_Time     DATETIME DEFAULT GETDATE(),    -- 建立日期
    Update_Time     DATETIME DEFAULT GETDATE(),    -- 修改日期
    Is_Active       CHAR(1) DEFAULT 'Y',           -- 是否啟用
);

-- 相簿上傳檔案
CREATE TABLE Album_FileUploads (
    File_Id         INT PRIMARY KEY IDENTITY(1,1), -- 檔案代號
    Group_Id        INT,                           -- 群組代號
    File_Real_Name  NVARCHAR(255) NOT NULL,        -- 原始檔名
    File_Save_Name  NVARCHAR(255) NOT NULL,        -- 存入磁碟檔名
    Content_Type    NVARCHAR(100) DEFAULT '',      -- 檔案類型 (MIME type)
    File_Path       NVARCHAR(MAX),                 -- 檔案路徑
    Upload_Time     DATETIME DEFAULT GETDATE(),    -- 上傳日期
    Update_Time     DATETIME DEFAULT GETDATE(),    -- 修改日期
    Is_Active       CHAR(1) DEFAULT 'Y',           -- 是否啟用
);