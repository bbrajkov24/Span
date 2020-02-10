CREATE PROCEDURE [span].[sp_GetPodaci]
AS
BEGIN
SELECT Ime, Prezime, PBr, Grad, Telefon
FROM [span].[Podaci]
END
GO