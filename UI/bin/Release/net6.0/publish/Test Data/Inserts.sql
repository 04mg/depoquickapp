INSERT INTO DepoQuickApp.dbo.Payments (Amount,Status) VALUES
	 (0.0,1),
	 (4662.0,1),
	 (150.0,1),
	 (0.0,0),
	 (19152.0,1),
	 (1302.0,1),
	 (15873.75,0),
	 (14917.5,0),
	 (20591.25,1),
	 (8887.5,0);
INSERT INTO DepoQuickApp.dbo.Payments (Amount,Status) VALUES
	 (4927.5,0),
	 (7425.0,1),
	 (36697.5,0),
	 (178.5,0),
	 (18270.0,1),
	 (0.0,1),
	 (0.0,1),
	 (1134.0,0),
	 (175.0,0),
	 (27.5,0);
INSERT INTO DepoQuickApp.dbo.Promotions (Label,Validity_Id,Validity_StartDate,Validity_EndDate,Discount) VALUES
	 (N'5 percent',0,'2024-06-10','2025-06-11',5),
	 (N'15 percent',0,'2024-06-10','2025-06-11',15),
	 (N'25 percent',0,'2024-06-10','2025-06-11',25),
	 (N'35 percent',0,'2024-06-10','2025-06-11',35),
	 (N'45 percent',0,'2024-06-10','2025-06-11',45),
	 (N'55 percent',0,'2024-06-10','2025-06-11',55),
	 (N'70 percent',0,'2024-06-10','2025-06-11',70),
	 (N'20 percent',0,'2024-06-10','2025-06-11',20),
	 (N'50 percent',0,'2024-06-10','2025-06-11',50);
INSERT INTO DepoQuickApp.dbo.Users (Email,Password,NameSurname,[Rank]) VALUES
	 (N'admin@test.com',N'1234567@Ad',N'Admin User',1),
	 (N'client@test.com',N'1234567@Cl',N'User One',0),
	 (N'user2@test.com',N'1234567@Cl',N'User Two',0);
INSERT INTO DepoQuickApp.dbo.Deposits (Name,ClimateControl,Area,[Size]) VALUES
	 (N'DepositLargeFalse',0,4,1),
	 (N'DepositLargeTrue',1,2,2),
	 (N'DepositMediumFalse',0,1,1),
	 (N'DepositMediumFalseOutdated',0,4,1),
	 (N'DepositSmallFalse',0,3,0),
	 (N'DepositSmallTrue',1,0,0);
INSERT INTO DepoQuickApp.dbo.DepositPromotion (DepositName,PromotionsId) VALUES
	 (N'DepositMediumFalse',1),
	 (N'DepositSmallTrue',2),
	 (N'DepositLargeFalse',5),
	 (N'DepositSmallFalse',5),
	 (N'DepositLargeFalse',6),
	 (N'DepositLargeTrue',6),
	 (N'DepositLargeFalse',7);
INSERT INTO DepoQuickApp.dbo.Deposits_AvailablePeriods (StartDate,EndDate,AvailabilityPeriodsDepositName) VALUES
	 ('2024-06-10','2024-06-25',N'DepositLargeFalse'),
	 ('2024-06-10','2025-01-19',N'DepositLargeTrue'),
	 ('2024-06-10','2025-01-21',N'DepositMediumFalse'),
	 ('2024-06-10','2024-11-19',N'DepositSmallFalse'),
	 ('2026-05-11','2027-12-18',N'DepositSmallTrue'),
	 ('2024-07-08','2024-10-22',N'DepositLargeFalse'),
	 ('2025-05-12','2025-09-24',N'DepositLargeTrue'),
	 ('2025-02-02','2026-05-04',N'DepositLargeFalse'),
	 ('2026-12-26','2027-01-04',N'DepositLargeTrue'),
	 ('2028-10-14','2028-12-11',N'DepositLargeTrue');
INSERT INTO DepoQuickApp.dbo.Deposits_AvailablePeriods (StartDate,EndDate,AvailabilityPeriodsDepositName) VALUES
	 ('2025-09-29','2026-02-05',N'DepositMediumFalse'),
	 ('2026-09-29','2027-02-11',N'DepositMediumFalse'),
	 ('2028-01-02','2028-12-11',N'DepositMediumFalse'),
	 ('2025-12-21','2025-12-22',N'DepositSmallFalse'),
	 ('2027-06-28','2027-12-11',N'DepositSmallFalse'),
	 ('2026-11-19','2026-11-19',N'DepositSmallFalse'),
	 ('2027-12-23','2027-12-27',N'DepositSmallTrue'),
	 ('2028-12-11','2028-12-11',N'DepositSmallTrue'),
	 ('2027-02-12','2027-02-18',N'DepositLargeFalse'),
	 ('2028-02-23','2028-12-11',N'DepositLargeFalse');
INSERT INTO DepoQuickApp.dbo.Deposits_AvailablePeriods (StartDate,EndDate,AvailabilityPeriodsDepositName) VALUES
	 ('2027-02-02','2028-09-11',N'DepositLargeTrue'),
	 ('2027-12-20','2027-12-21',N'DepositSmallFalse'),
	 ('2027-12-24','2028-12-11',N'DepositSmallFalse');
INSERT INTO DepoQuickApp.dbo.Deposits_UnavailablePeriods (StartDate,EndDate,AvailabilityPeriodsDepositName) VALUES
	 ('2024-06-26','2024-07-07',N'DepositLargeFalse'),
	 ('2025-01-20','2025-05-11',N'DepositLargeTrue'),
	 ('2024-06-10','2024-06-12',N'DepositMediumFalseOutdated'),
	 ('2024-10-23','2025-02-01',N'DepositLargeFalse'),
	 ('2025-09-25','2026-12-25',N'DepositLargeTrue'),
	 ('2028-09-12','2028-10-13',N'DepositLargeTrue'),
	 ('2025-01-22','2025-09-28',N'DepositMediumFalse'),
	 ('2026-02-06','2026-09-28',N'DepositMediumFalse'),
	 ('2027-02-12','2028-01-01',N'DepositMediumFalse'),
	 ('2024-11-20','2025-12-20',N'DepositSmallFalse');
INSERT INTO DepoQuickApp.dbo.Deposits_UnavailablePeriods (StartDate,EndDate,AvailabilityPeriodsDepositName) VALUES
	 ('2026-11-20','2027-06-27',N'DepositSmallFalse'),
	 ('2025-12-23','2026-11-18',N'DepositSmallFalse'),
	 ('2024-06-10','2026-05-10',N'DepositSmallTrue'),
	 ('2027-12-19','2027-12-22',N'DepositSmallTrue'),
	 ('2027-12-28','2028-12-10',N'DepositSmallTrue'),
	 ('2026-05-05','2027-02-11',N'DepositLargeFalse'),
	 ('2027-02-19','2028-02-22',N'DepositLargeFalse'),
	 ('2027-01-05','2027-02-01',N'DepositLargeTrue'),
	 ('2027-12-12','2027-12-19',N'DepositSmallFalse'),
	 ('2027-12-22','2027-12-23',N'DepositSmallFalse');
INSERT INTO DepoQuickApp.dbo.Bookings (DepositName,ClientEmail,Message,Stage,PaymentId,Duration_Id,Duration_StartDate,Duration_EndDate) VALUES
	 (N'DepositLargeFalse',N'admin@test.com',N'',0,1,0,'2024-06-26','2024-07-07'),
	 (N'DepositLargeTrue',N'admin@test.com',N'',0,2,0,'2025-01-20','2025-05-11'),
	 (N'DepositMediumFalseOutdated',N'client@test.com',N'',0,3,0,'2024-06-10','2024-06-12'),
	 (N'DepositLargeFalse',N'client@test.com',N'',1,4,0,'2024-10-23','2025-02-01'),
	 (N'DepositLargeTrue',N'client@test.com',N'',0,5,0,'2025-09-25','2026-12-25'),
	 (N'DepositLargeTrue',N'client@test.com',N'',0,6,0,'2028-09-12','2028-10-13'),
	 (N'DepositMediumFalse',N'client@test.com',N'',1,7,0,'2025-01-22','2025-09-28'),
	 (N'DepositMediumFalse',N'client@test.com',N'',1,8,0,'2026-02-06','2026-09-28'),
	 (N'DepositMediumFalse',N'client@test.com',N'',0,9,0,'2027-02-12','2028-01-01'),
	 (N'DepositSmallFalse',N'client@test.com',N'',1,10,0,'2024-11-20','2025-12-20');
INSERT INTO DepoQuickApp.dbo.Bookings (DepositName,ClientEmail,Message,Stage,PaymentId,Duration_Id,Duration_StartDate,Duration_EndDate) VALUES
	 (N'DepositSmallFalse',N'client@test.com',N'',1,11,0,'2026-11-20','2027-06-27'),
	 (N'DepositSmallFalse',N'client@test.com',N'',0,12,0,'2025-12-23','2026-11-18'),
	 (N'DepositSmallTrue',N'client@test.com',N'',1,13,0,'2024-06-10','2026-05-10'),
	 (N'DepositSmallTrue',N'client@test.com',N'',1,14,0,'2027-12-19','2027-12-22'),
	 (N'DepositSmallTrue',N'client@test.com',N'',0,15,0,'2027-12-28','2028-12-10'),
	 (N'DepositLargeFalse',N'user2@test.com',N'',0,16,0,'2026-05-05','2027-02-11'),
	 (N'DepositLargeFalse',N'user2@test.com',N'',0,17,0,'2027-02-19','2028-02-22'),
	 (N'DepositLargeTrue',N'user2@test.com',N'',1,18,0,'2027-01-05','2027-02-01'),
	 (N'DepositSmallFalse',N'user2@test.com',N'',1,19,0,'2027-12-12','2027-12-19'),
	 (N'DepositSmallFalse',N'user2@test.com',N'',1,20,0,'2027-12-22','2027-12-23');
