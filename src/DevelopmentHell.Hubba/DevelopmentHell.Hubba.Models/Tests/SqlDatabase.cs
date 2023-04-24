﻿namespace DevelopmentHell.Hubba.Models.Tests
{
    public enum Databases
    {
        USERS,
        LOGS,
        NOTIFICATIONS,
        COLLABORATOR_PROFILES,
        LISTING_PROFILES
    }

    public enum Tables
    {
        RECOVERY_REQUESTS, USER_ACCOUNTS, USER_LOGINS, USER_OTPS,
        LOGS,
        NOTIFICATION_SETTINGS, USER_NOTIFICATIONS,
        COLLABORATOR_FILE_JUNCTION, COLLABORATORS, COLLABORATOR_FILES, USER_VOTES,
        LISTINGS, LISTING_HISTORY, LISTING_RATINGS, LISTING_AVAILABILITIES, BOOKINGS, BOOKEDTIMEFRAMES
    }
}
