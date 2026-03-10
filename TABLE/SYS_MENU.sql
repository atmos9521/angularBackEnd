-- 1. 選單主表
CREATE TABLE Sys_Menus (
    Menu_Id INT PRIMARY KEY IDENTITY(1,1),
    Group_Id INT DEFAULT NULL,           -- 群組ID
    Parent_Id INT NULL,                 -- 父選單 ID (NULL 代表第一層)
    Label NVARCHAR(100) NOT NULL,       -- 選單顯示文字
    Icon NVARCHAR(50) NULL,             -- PrimeIcons 代碼 (如 pi-home)
    RouterLink NVARCHAR(200) NULL,      -- Angular 路由路徑
    Sort_Order INT DEFAULT 0,           -- 排序用
    Is_Active CHAR(1) DEFAULT 'Y',      -- 是否啟用
    CONSTRAINT FK_Menu_Parent FOREIGN KEY (Parent_Id) REFERENCES Sys_Menus(Menu_Id)
);


-- 插入父層選單
INSERT INTO Sys_Menus (Label, Icon, RouterLink, Sort_Order) VALUES (N'Home', N'pi pi-home', '/home', 1);
INSERT INTO Sys_Menus (Label, Icon, RouterLink, Sort_Order) VALUES (N'坂道', N'pi pi-search', NULL, 2);
INSERT INTO Sys_Menus (Label, Icon, RouterLink, Sort_Order) VALUES (N'關於', N'pi pi-search', NULL, 3);

-- 插入 Projects 的子選單 (假設 Projects 的 Menu_Id 是 2)
INSERT INTO Sys_Menus (Parent_Id, Label, Icon, RouterLink, Sort_Order) VALUES (2, N'乃木坂', N'pi pi-palette', ''   , 1);
INSERT INTO Sys_Menus (Parent_Id, Label, Icon, RouterLink, Sort_Order) VALUES (2, N'櫻坂'  , N'pi pi-palette', '' , 2);
INSERT INTO Sys_Menus (Parent_Id, Label, Icon, RouterLink, Sort_Order) VALUES (2, N'日向坂', N'pi pi-palette', '' , 3);

-- 插入 Templates 的子選單 (假設 Templates 的 Menu_Id 是 4)
INSERT INTO Sys_Menus (Parent_Id, Label, Icon, RouterLink, Sort_Order) 
VALUES (4, N'成員', N'pi pi-palette', '/idol/nogizaka46/nogizaka46Member', 1);

-- 2. 群組表單
CREATE TABLE Sys_Menu_Group (
    Group_Id INT PRIMARY KEY IDENTITY(1,1),
    Group_Name NVARCHAR(100) NOT NULL,      -- 選單顯示文字
    Is_Active CHAR(1) DEFAULT 'Y',          -- 是否啟用
);

INSERT INTO Sys_Menu_Group (Group_Name) VALUES (N'偶像');

-- -- 2. 角色表
-- CREATE TABLE Sys_Roles (
    -- Role_Id INT PRIMARY KEY IDENTITY(1,1),
    -- Role_Name NVARCHAR(50) NOT NULL     -- Admin, Staff, Guest...
-- );

-- -- 3. 角色與選單對應表 (權限設定)
-- CREATE TABLE Sys_Role_Menu (
    -- Role_Id INT NOT NULL,
    -- Menu_Id INT NOT NULL,
    -- PRIMARY KEY (Role_Id, Menu_Id),
    -- FOREIGN KEY (Role_Id) REFERENCES Sys_Roles(Role_Id),
    -- FOREIGN KEY (Menu_Id) REFERENCES Sys_Menus(Menu_Id)
-- );