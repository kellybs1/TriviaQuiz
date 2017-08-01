CREATE TABLE tblPlayers 
(
	PlayerID		int		IDENTITY,
	PlayerName		varchar(25)	NOT NULL,
	PlayerPassword	varchar(25)	NOT NULL,

	CONSTRAINT PK_Player PRIMARY KEY(PlayerID)
);