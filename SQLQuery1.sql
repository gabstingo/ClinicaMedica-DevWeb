INSERT INTO Logins (Ativo) VALUES (0);

select * from Administrador

select * from Logins

select * from Consulta

INSERT INTO Logins(Email,Senha,Crm,Cpf,Nome,DataNasc,TipoUsuario,PrimeiroAcesso,Ativo) 
VALUES ('admin@admin','998ed4d621742d0c2d85ed84173db569afa194d4597686cae947324aa58ab4bb','0','0','Administrador','2000-01-01','Administrador',1,1);

DELETE from Logins WHERE ID=3;

DELETE from Consulta WHERE ID=1;

ALTER TABLE Logins DROP COLUMN Logado;

UPDATE  Consulta  
SET Situacao = 'Confirmada'
WHERE ID = '1';

--select Email,Senha

--from Logins

--where 