CREATE TYPE [span].[PodaciDTO] AS TABLE(
	[Ime] [nvarchar](50) NOT NULL,
	[Prezime] [nvarchar](50) NOT NULL,
	[PBr] [nvarchar](20) NOT NULL,
	[Grad] [nvarchar](50) NOT NULL,
	[Telefon] [nvarchar](20) NOT NULL
)
GO