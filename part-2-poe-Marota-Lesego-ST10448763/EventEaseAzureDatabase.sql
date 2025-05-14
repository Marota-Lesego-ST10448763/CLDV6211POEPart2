DROP TABLE IF EXISTS Booking;
DROP TABLE IF EXISTS Event;
DROP TABLE IF EXISTS Venue;



--CREATE VENUE TABLE
CREATE TABLE Venue (
     VenueID INT IDENTITY (1,1) PRIMARY KEY,
	 VenueName NVARCHAR (255) NOT NULL,
	 Location NVARCHAR (255) NOT NULL,
	 Capacity INT NOT NULL CHECK (Capacity > 0),
	 ImageUrl NVARCHAR (500) NULL
);

--CREATE EVENT TABLE
CREATE TABLE Event (
     EventID INT IDENTITY (1,1) PRIMARY KEY,
	 EventName NVARCHAR (255) NOT NULL,
	 EventDate DATETIME NOT NULL,
	 Description  NVARCHAR(MAX) NULL,
     VenueID INT NULL,
	 FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE SET NULL
);

--CREATE BOOKING TABLE (ASSOCIATIVE TABLE)
CREATE TABLE Booking (
    BookingID INT IDENTITY (1,1) PRIMARY KEY,
	EventID INT NOT NULL,
	VenueID INT NOT NULL,
	BookingDate DATETIME DEFAULT GETDATE(),
	FOREIGN KEY (EventID) REFERENCES Event(EventID) ON DELETE CASCADE,
	FOREIGN KEY (VenueID) REFERENCES Venue(VenueID) ON DELETE CASCADE,
	CONSTRAINT UQ_Booking UNIQUE (EventID, VenueID)
);

-- Insert sample data into Venue table
INSERT INTO Venue (VenueName, Location, Capacity, ImageUrl)
VALUES
('Sunset Banquet Hall', '101 Sunset Blvd, Cape Town', 350, 'https://placeholder.com/sunset.jpg'),
('Ocean View Conference Center', '222 Coastal Road, Durban', 450, 'https://placeholder.com/oceanview.jpg'),
('Mountain Retreat Lodge', '333 Highland St, Johannesburg', 250, 'https://placeholder.com/mountain.jpg');

-- Insert sample data into Event table
INSERT INTO Event (EventName, EventDate, Description, VenueID)
VALUES
('AI & Robotics Summit', '2025-09-12 08:30:00', 'A conference focused on the latest AI and robotics advancements.', 1),
('Jazz Night Gala', '2025-10-05 19:00:00', 'An elegant evening of live jazz performances and fine dining.', 2),
('Startup Pitch Competition', '2025-11-20 14:00:00', 'Entrepreneurs showcase their startups to investors and judges.', 3);

-- Insert sample data into Booking table
INSERT INTO Booking (EventID, VenueID, BookingDate) 
VALUES
(1, 1, '2025-08-25 11:00:00'),
(2, 2, '2025-09-15 16:45:00'),
(3, 3, '2025-10-30 13:20:00');

SELECT * FROM Venue;
SELECT * FROM Event;
SELECT * FROM Booking;