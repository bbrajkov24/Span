CREATE TABLE [span].[Podaci](
	[PodaciID] [int] IDENTITY(1,1) NOT NULL,
	[Ime] [nvarchar](50) NOT NULL,
	[Prezime] [nvarchar](50) NOT NULL,
	[PBr] [nvarchar](20) NOT NULL,
	[Grad] [nvarchar](50) NOT NULL,
	[Telefon] [nvarchar](20) NOT NULL

CONSTRAINT [Podaci_PK] PRIMARY KEY CLUSTERED 
(
	[PodaciID] ASC
)ON [PRIMARY],

CONSTRAINT [Podaci_UC] UNIQUE (Ime, Prezime, PBr, Grad, Telefon)

) ON [PRIMARY]

GO