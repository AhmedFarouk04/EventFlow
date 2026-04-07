IF DB_ID('EventDrivenBookingDB') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingDB;
END;
GO

IF DB_ID('EventDrivenBookingAvailabilityDb') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingAvailabilityDb;
END;
GO

IF DB_ID('EventDrivenBookingAuditDb') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingAuditDb;
END;
GO

IF DB_ID('EventDrivenBookingPricingDb') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingPricingDb;
END;
GO

IF DB_ID('EventDrivenBookingNotificationsDb') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingNotificationsDb;
END;
GO

IF DB_ID('EventDrivenBookingUsersDb') IS NULL
BEGIN
    CREATE DATABASE EventDrivenBookingUsersDb;
END;
GO
