-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;

-- DepoQuickApp.dbo.Promotions definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Promotions;

CREATE TABLE DepoQuickApp.dbo.Promotions (
	Id int IDENTITY(1,1) NOT NULL,
	Label nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Validity_Id int NOT NULL,
	Validity_StartDate date NOT NULL,
	Validity_EndDate date NOT NULL,
	Discount int NOT NULL,
	CONSTRAINT PK_Promotions PRIMARY KEY (Id)
);


-- DepoQuickApp.dbo.Deposits definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Deposits;

CREATE TABLE DepoQuickApp.dbo.Deposits (
	Name nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ClimateControl bit NOT NULL,
	Area int NOT NULL,
	[Size] int NOT NULL,
	CONSTRAINT PK_Deposits PRIMARY KEY (Name)
);


-- DepoQuickApp.dbo.Payments definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Payments;

CREATE TABLE DepoQuickApp.dbo.Payments (
	Id int IDENTITY(1,1) NOT NULL,
	Amount float NOT NULL,
	Status int NOT NULL,
	CONSTRAINT PK_Payments PRIMARY KEY (Id)
);

-- DepoQuickApp.dbo.Users definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Users;

CREATE TABLE DepoQuickApp.dbo.Users (
	Email nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Password nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	NameSurname nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Rank] int NOT NULL,
	CONSTRAINT PK_Users PRIMARY KEY (Email)
);


-- DepoQuickApp.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.[__EFMigrationsHistory];

CREATE TABLE DepoQuickApp.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- DepoQuickApp.dbo.Bookings definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Bookings;

CREATE TABLE DepoQuickApp.dbo.Bookings (
	Id int IDENTITY(1,1) NOT NULL,
	DepositName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ClientEmail nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Message nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Stage int NOT NULL,
	PaymentId int NULL,
	Duration_Id int NOT NULL,
	Duration_StartDate date NOT NULL,
	Duration_EndDate date NOT NULL,
	CONSTRAINT PK_Bookings PRIMARY KEY (Id),
	CONSTRAINT FK_Bookings_Deposits_DepositName FOREIGN KEY (DepositName) REFERENCES DepoQuickApp.dbo.Deposits(Name) ON DELETE CASCADE,
	CONSTRAINT FK_Bookings_Payments_PaymentId FOREIGN KEY (PaymentId) REFERENCES DepoQuickApp.dbo.Payments(Id),
	CONSTRAINT FK_Bookings_Users_ClientEmail FOREIGN KEY (ClientEmail) REFERENCES DepoQuickApp.dbo.Users(Email) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Bookings_ClientEmail ON dbo.Bookings (  ClientEmail ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Bookings_DepositName ON dbo.Bookings (  DepositName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Bookings_PaymentId ON dbo.Bookings (  PaymentId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- DepoQuickApp.dbo.DepositPromotion definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.DepositPromotion;

CREATE TABLE DepoQuickApp.dbo.DepositPromotion (
	DepositName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	PromotionsId int NOT NULL,
	CONSTRAINT PK_DepositPromotion PRIMARY KEY (DepositName,PromotionsId),
	CONSTRAINT FK_DepositPromotion_Deposits_DepositName FOREIGN KEY (DepositName) REFERENCES DepoQuickApp.dbo.Deposits(Name) ON DELETE CASCADE,
	CONSTRAINT FK_DepositPromotion_Promotions_PromotionsId FOREIGN KEY (PromotionsId) REFERENCES DepoQuickApp.dbo.Promotions(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_DepositPromotion_PromotionsId ON dbo.DepositPromotion (  PromotionsId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- DepoQuickApp.dbo.Deposits_AvailablePeriods definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Deposits_AvailablePeriods;

CREATE TABLE DepoQuickApp.dbo.Deposits_AvailablePeriods (
	Id int IDENTITY(1,1) NOT NULL,
	StartDate date NOT NULL,
	EndDate date NOT NULL,
	AvailabilityPeriodsDepositName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Deposits_AvailablePeriods PRIMARY KEY (Id),
	CONSTRAINT FK_Deposits_AvailablePeriods_Deposits_AvailabilityPeriodsDepositName FOREIGN KEY (AvailabilityPeriodsDepositName) REFERENCES DepoQuickApp.dbo.Deposits(Name) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Deposits_AvailablePeriods_AvailabilityPeriodsDepositName ON dbo.Deposits_AvailablePeriods (  AvailabilityPeriodsDepositName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- DepoQuickApp.dbo.Deposits_UnavailablePeriods definition

-- Drop table

-- DROP TABLE DepoQuickApp.dbo.Deposits_UnavailablePeriods;

CREATE TABLE DepoQuickApp.dbo.Deposits_UnavailablePeriods (
	Id int IDENTITY(1,1) NOT NULL,
	StartDate date NOT NULL,
	EndDate date NOT NULL,
	AvailabilityPeriodsDepositName nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Deposits_UnavailablePeriods PRIMARY KEY (Id),
	CONSTRAINT FK_Deposits_UnavailablePeriods_Deposits_AvailabilityPeriodsDepositName FOREIGN KEY (AvailabilityPeriodsDepositName) REFERENCES DepoQuickApp.dbo.Deposits(Name) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Deposits_UnavailablePeriods_AvailabilityPeriodsDepositName ON dbo.Deposits_UnavailablePeriods (  AvailabilityPeriodsDepositName ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;