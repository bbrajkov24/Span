IF OBJECT_ID('span.sp_InsertPodaci', 'P') IS NOT NULL
  DROP PROCEDURE span.sp_InsertPodaci;
  GO
CREATE PROCEDURE span.sp_InsertPodaci @InsertPodaci span.PodaciDTO READONLY
  , @ErrorMessage NVARCHAR(2000) OUTPUT
AS
BEGIN
  SET NOCOUNT ON
  SET XACT_ABORT ON
  BEGIN TRY
    DECLARE @rowCount INT;
    BEGIN TRANSACTION SavePoint;
    INSERT INTO span.Podaci (
      Ime
      , Prezime
      , PBr
      , Grad
      , Telefon
      )
    SELECT Ime
      , Prezime
      , PBr
      , Grad
      , Telefon
    FROM @InsertPodaci new
    WHERE NOT EXISTS (
        SELECT PodaciID
        FROM span.Podaci old
        WHERE old.Ime = new.Ime
          AND old.Prezime = new.Prezime
          AND old.PBr = new.PBr
          AND old.Grad = new.Grad
          AND old.Telefon = new.Telefon
        )
    SELECT @rowCount = @@ROWCOUNT
    COMMIT TRANSACTION;
    SELECT @ErrorMessage = CONVERT(NVARCHAR(100), @rowCount) + ' unique rows inserted!'
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
      SELECT @ErrorMessage = ERROR_MESSAGE()
      EXEC xp_logevent 70000
        , @ErrorMessage
        , ERROR
      ROLLBACK TRANSACTION SavePoint;
    END
  END CATCH
  RETURN
END
GO
