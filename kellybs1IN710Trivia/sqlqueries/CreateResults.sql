CREATE TABLE tblResults
(
	ResultID		int IDENTITY,
	PlayerID		int NOT NULL,
	Score			int	NOT NULL,
	ResultDateTime	datetime NOT NULL,

	CONSTRAINT PK_Results PRIMARY KEY (ResultID)
);