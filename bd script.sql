--пересоздать
 DROP SCHEMA public CASCADE;
 CREATE SCHEMA public;

--сброс счетчиков
do $$DECLARE
	r RECORD;
begin
	for r in (select sequence_name from information_schema.sequences where sequence_schema = 'public')loop
		execute 'ALTER SEQUENCE' || r.sequence_name || 'RESTART WITH 1';
	end loop;	
end$$;

CREATE TABLE Статус 
(Код_статуса int PRIMARY KEY GENERATED ALWAYS AS IDENTITY, 
 Название char(255) NOT NULL
);


CREATE TABLE Этапы_обработки 
(Код_этапа int PRIMARY KEY GENERATED ALWAYS AS IDENTITY, 
 Название char(255) NOT NULL
);


CREATE TABLE Поставщик 
(Код_поставщика int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Фамилия char(255) NOT NULL,
 Имя char(255) NOT NULL,
 Отчество char(255) NOT NULL,
 Адрес char(255) NOT NULL,
 Номер_телефона char(255) NOT NULL,
 Название char(255) NOT NULL
);


CREATE TABLE Клиент 
(Код_клиента int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Фамилия char(255) NOT NULL,
 Имя char(255) NOT NULL,
 Отчество char(255) NOT NULL,
 Адрес char(255) NOT NULL,
 Номер_телефона char(255) NOT NULL
);

CREATE TABLE Материал 
(Код_материала int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Единица_измерения char(255) NOT NULL,
 Количество_в_листах int NOT NULL,
 Название char(255) NOT NULL
);

CREATE TABLE Изделие 
(Код_изделия int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Стоимость int NOT NULL,
 Шаблон char(255) NOT NULL
);

CREATE TABLE Заказ 
(Код_заказа int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Код_клиента int NOT NULL,FOREIGN KEY(Код_клиента) REFERENCES Клиент(Код_клиента),
 Код_статуса int NOT NULL,FOREIGN KEY(Код_статуса) REFERENCES Статус(Код_статуса),
 Дата_заказа date NOT NULL,
 Сумма_заказа int default null
);

CREATE TABLE Связь_заказ_изделие
(Код_связь_заказ_изделие int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Код_заказа int NOT NULL,FOREIGN KEY(Код_заказа) REFERENCES Заказ(Код_заказа),
 Код_изделия int NOT NULL,FOREIGN KEY(Код_изделия) REFERENCES Изделие(Код_изделия),
 Количество_в_листах int NOT null
);

CREATE TABLE Заготовка 
(Код_заготовки int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Количество int default null,
 Код_связь_заказ_изделие int not null,FOREIGN KEY(Код_связь_заказ_изделие) REFERENCES Связь_заказ_изделие(Код_связь_заказ_изделие),
 Размер_см_кв int NOT NULL
);

CREATE TABLE Поставка_поставщик
(Код_поставки int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Код_поставщика int NOT NULL,FOREIGN KEY(Код_поставщика) REFERENCES Поставщик(Код_поставщика),
 Сумма_поставки int NOT NULL,
 Дата_поставки date NOT NULL
);

CREATE TABLE Поставка_материал
(Код_поставки_материал int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Код_поставки int NOT NULL,FOREIGN KEY(Код_поставки) REFERENCES Поставка_поставщик(Код_поставки),
 Количество_в_листах int NOT null,
 Код_материала int NOT null,FOREIGN KEY(Код_материала) REFERENCES Материал(Код_материала)
);


CREATE TABLE Связь_материал_заготовка
(Код_связь_материал_заготовка int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Количество_в_заготовках int default null,
 Количество_в_листах int default null,
 Дата date not null,
 Код_материала int NOT NULL,FOREIGN KEY(Код_материала) REFERENCES Материал(Код_материала),
 Код_заготовки int NOT NULL,FOREIGN KEY(Код_заготовки) REFERENCES Заготовка(Код_заготовки)
);

CREATE TABLE Обработка
(Код_обработки int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Код_заготовки int NOT NULL,FOREIGN KEY(Код_заготовки) REFERENCES Заготовка(Код_заготовки),
 Код_этапа int NOT NULL,FOREIGN KEY(Код_этапа) REFERENCES Этапы_обработки(Код_этапа),
 Количетсво int default null,
 Дата date NOT NULL
);

CREATE TABLE Администратор
(Код_администратор int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
 Логин char(50) NOT NULL,
 Пароль char(50) NOT NULL,
 Номер_телефона char(30) NOT NULL,
 Почта char(50) NOT NULL,
 Последний_вход timestamp ,
 Неверный_пароль_счетчик int,
 Роль char(20) 
);

Select * from Администратор;
--------------------------ТРИГГЕРЫ
--1.Конвертировать количество материала в Количество_в_листах
CREATE OR REPLACE FUNCTION update_material_quantity_material()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Связь_материал_заготовка
    SET Количество_в_листах = NEW.Количество_в_листах
    WHERE Код_материала = NEW.Код_материала;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_material_quantity_material_trigger
AFTER INSERT OR UPDATE OF Количество_в_листах
ON Материал
FOR EACH ROW
EXECUTE FUNCTION update_material_quantity_material();

select * from Связь_материал_заготовка;
select * from Материал;

--2.конвертирует значения Количество в таблице Связь_материал_заготовка в Количество_в_заготовках(та же таблица) по формуле Количество_в_заготовках=Количество*20,
--обновляет значения и применяет формулы после изменений в столбце Количество
CREATE OR REPLACE FUNCTION update_material_quantity()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Связь_материал_заготовка
    SET Количество_в_заготовках = NEW.Количество_в_листах * 20
    WHERE Код_материала = NEW.Код_материала;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_material_quantity_trigger
AFTER INSERT OR UPDATE OF Количество_в_листах
ON Связь_материал_заготовка
FOR EACH ROW
EXECUTE FUNCTION update_material_quantity();



-- .при добавлении Заказа в таблице Заказ изменяет Количество_в_заготовках на Количество в таблице Связь_заказ_изделие(он уменьшает количество имеющихся заготовок на то число,которое заказали)
CREATE OR REPLACE FUNCTION trg_update_material_quantity_postavka()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE Материал
  SET Количество_в_листах = Количество_в_листах - NEW.Количество_в_листах
  WHERE Материал.Код_материала = NEW.Код_материала;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_material_quantity_postavka
AFTER UPDATE OF Количество_в_листах ON Поставка_материал
FOR EACH ROW
EXECUTE FUNCTION trg_update_material_quantity_postavka();

--4.Списать заготовку при поступлении заказа(отправить ее в обработку)
CREATE OR REPLACE FUNCTION trg_update_material_quantity_order()
RETURNS TRIGGER AS $$
BEGIN
  UPDATE Связь_материал_заготовка
  SET Количество_в_листах = Количество_в_листах - (SELECT Количество_в_листах FROM Связь_заказ_изделие WHERE Код_заказа = NEW.Код_заказа)
  WHERE Код_заготовки = (SELECT Код_заготовки FROM Связь_заказ_изделие WHERE Код_заказа = NEW.Код_заказа);
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_update_material_quantity_order
AFTER INSERT ON Заказ
FOR EACH ROW
EXECUTE FUNCTION trg_update_material_quantity_order();


--5.Рассчитать Сумму заказа
CREATE OR REPLACE FUNCTION update_order_amount()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Заказ
    SET Сумма_заказа = (SELECT Количество_в_листах * Стоимость
                        FROM Связь_заказ_изделие
                        JOIN Изделие ON Связь_заказ_изделие.Код_изделия = Изделие.Код_изделия
                        WHERE Связь_заказ_изделие.Код_связь_заказ_изделие = NEW.Код_связь_заказ_изделие)
    WHERE Код_заказа = NEW.Код_заказа;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_order_amount_trigger
AFTER INSERT ON Связь_заказ_изделие
FOR EACH ROW
EXECUTE FUNCTION update_order_amount();

select * from Заказ;


-------------------------------ЗАПОЛНЕНИЕ
INSERT INTO Статус (Название) VALUES
('Ожидает оплаты'),
('Подтвержден'),
('Изготовляется'),
('Отправлен'),
('Доставляется'),
('Готов к получению'),
('Завершен'),
('Отменен');

INSERT INTO Этапы_обработки (Название) VALUES
('Лазерная резка заготовки'),
('Обработка заготовки'),
('УФ печать');

INSERT INTO Поставщик (Фамилия,Имя,Отчество,Адрес,Номер_телефона,Название) VALUES
('Данилов', 'Данил', 'Данилович','Данилова,5','8-987-248-25-50','1 отец'),
('Владимир', 'Владимирович', 'Здраствуйте','Владимирова,15','8-917-202-50-40','2 ОТЦА И 2 СЫНА');


select * from Клиент;
INSERT INTO Клиент (Фамилия,Имя,Отчество,Адрес,Номер_телефона) VALUES
('Антонов', 'Антон' ,'Антонович','Антоновка, 15','8-987-356-20-50'),
('Симонов', 'Олег' ,'Владленович','Домодедовская, 94','8-909-625-42-56'),
('Селезнёв','Лазарь' ,'Тимофеевич','Ладыгина, 87','8-987-356-20-50'),
('Кириллов', 'Панкратий', 'Кириллович','Косиора, 87','8-987-356-20-50'),
('Колобов', 'Вениамин' ,'Сергеевич','Пионерская, 22 ','8-987-356-20-50'),
('Мартынов', 'Аристарх', 'Глебович','Антоновка,15','8-987-356-20-50'),
('Жданов', 'Мартин', 'Владиславович','Антоновка,15','8-987-356-20-50'),
('Лебедев', 'Климент', 'Владиславович','Антоновка,15','8-987-356-20-50'),
('Журавлёв', 'Адриан', 'Васильевич','Антоновка,15','8-987-356-20-50'),
('Лыткин', 'Самуил' ,'Игнатьевич','Антоновка,15','8-987-356-20-50');




insert into Материал (Единица_измерения,Количество_в_листах,Название) values 
('м2',10,'Оргстекло'),
('м2',5,'Акриловое стекло');

insert into Изделие (Стоимость,Шаблон) values 
(300,'https://clck.ru/36WfSb'),
(350,'https://clck.ru/32W65V'),
(300,'https://clck.ru/3856fB'),
(350,'https://clck.ru/SBGe5V'),
(350,'https://clck.ru/PS5pc8');

insert into Заказ (Код_клиента,Код_статуса,Дата_заказа,Сумма_заказа) values 
(1,2,'20.12.2022',null),
(2,1,'22.12.2022',null),
(3,3,'26.12.2022',null),
(4,4,'27.12.2022',null),
(5,2,'15.12.2022',null),
(6,6,'16.12.2022',null),
(7,5,'10.12.2022',null),
(8,1,'1.12.2022',null),
(9,2,'2.12.2022',null),
(10,2,'3.12.2022',null);


insert into Связь_заказ_изделие (Код_заказа,Код_изделия,Количество_в_листах) values 
(1,1,10),
(2,2,5),
(3,3,30),
(4,4,25),
(5,5,3),
(6,1,15),
(7,2,10),
(8,3,5),
(9,4,20),
(10,5,35),
(1,5,15);


insert into Заготовка (Количество,Размер_см_кв,Код_связь_заказ_изделие) values 
(10,1,1),
(25,2,2),
(50,2,3),
(20,1,4),
(25,1,5),
(50,2,6),
(20,2,7),
(25,1,8),
(10,1,9),
(15,2,10);

insert into Поставка_поставщик(Код_поставщика,Сумма_поставки,Дата_поставки) values
(1,50000,'30.12.2022'),
(2,20000,'28.12.2022'),
(2,35000,'27.12.2022'),
(1,30000,'22.12.2022'),
(1,10000,'15.12.2022'),
(2,15000,'10.12.2022'),
(1,30000,'8.12.2022'),
(2,5000,'9.12.2022'),
(1,10000,'1.12.2022'),
(2,15000,'2.12.2022');

insert into Поставка_материал (Код_поставки,Количество_в_листах,Код_материала) values 
(1,5,1),
(2,10,2),
(3,5,2),
(4,15,1),
(5,50,2),
(6,20,1),
(8,30,2),
(9,50,1),
(7,10,2),
(10,15,1);

insert into Связь_материал_заготовка (Количество_в_заготовках,Количество_в_листах,Дата,Код_материала,Код_заготовки) values 
(null,null,'30.12.2022',1,2),
(null,null,'28.12.2022',2,2),
(null,null,'4.12.2022',2,1),
(null,null,'15.12.2022',1,2),
(null,null,'16.12.2022',1,1),
(null,null,'10.12.2022',2,2),
(null,null,'3.12.2022',2,1),
(null,null,'20.12.2022',2,1),
(null,null,'21.12.2022',1,2),
(null,null,'1.12.2022',2,2);

insert into Обработка (Код_заготовки,Код_этапа,Количетсво,Дата) values 
(1,1,5,'1.01.2023'),
(2,2,15,'5.01.2023'),
(2,3,10,'3.01.2023'),
(1,1,25,'12.01.2023'),
(2,2,20,'4.01.2023'),
(2,3,20,'20.01.2023'),
(1,1,15,'21.01.2023'),
(1,2,10,'30.01.2023'),
(2,3,15,'12.01.2023'),
(1,2,15,'15.01.2023');



--test
select * from Заказ;
update Поставка_материал
set Количество_в_листах = 20
where Код_поставки_материал = 1;
update Поставка_материал
set Количество_в_листах = 10
where Код_поставки_материал = 2;
select * from Поставка_материал;
update Заказ
set Код_статуса = 5
where Код_заказа = 1;
update Заказ
set Код_статуса = 4
where Код_заказа = 3;



---------------------------------запросики
--1. Вывести список всех клиентов и сумму всех их заказов за последний год.
SELECT Клиент.Фамилия, Клиент.Имя, SUM(Заказ.Сумма_заказа) AS Общая_сумма
FROM Клиент
JOIN Заказ ON Клиент.Код_клиента = Заказ.Код_клиента
WHERE Заказ.Дата_заказа >= CURRENT_DATE - INTERVAL '1 year'
GROUP BY Клиент.Фамилия, Клиент.Имя;


--2. Показать все поставки материалов с указанием поставщика
SELECT Поставка_поставщик.Дата_поставки, Поставщик.Название
FROM Поставка_поставщик
JOIN Поставщик ON Поставка_поставщик.Код_поставщика = Поставщик.Код_поставщика
JOIN Поставка_материал ON Поставка_поставщик.Код_поставки = Поставка_материал.Код_поставки
JOIN Материал ON Поставка_материал.Код_материала = Материал.Код_материала
GROUP BY Поставка_поставщик.Дата_поставки, Поставщик.Название;


--3. Вывести список всех изделий и количество заказов, в которых они используются.
SELECT Изделие.Шаблон, COUNT(Связь_заказ_изделие.Код_заказа) AS Количество_заказов
FROM Изделие
LEFT JOIN Связь_заказ_изделие ON Изделие.Код_изделия = Связь_заказ_изделие.Код_изделия
GROUP BY Изделие.Шаблон;


--4. Найти все заказы, сделанные определенным клиентом за определенный период времени.
SELECT *
FROM Заказ
WHERE Код_клиента = 1 AND Дата_заказа BETWEEN '10.10.2020' AND '10.10.2023';


--5. Вывести список всех поставок материалов, сделанных определенным поставщиком за последний год.
SELECT *
FROM Поставка_поставщик
WHERE Код_поставщика = 1 AND Дата_поставки >= CURRENT_DATE - INTERVAL '1 year';


--6. Найти все заказы, в которых было использовано определенное количество изделий.
SELECT *
FROM Связь_заказ_изделие
WHERE Количество_в_листах < 15;


--7. Найти все заказы, находящиеся в определенном статусе.
SELECT *
FROM Заказ
WHERE Код_статуса = 2;


--8. Вывести список всех поставок материалов с указанием количества каждого материала.
SELECT Материал.Название, SUM(Поставка_материал.Количество_в_листах) AS Общее_количество_в_листах
FROM Материал
JOIN Поставка_материал ON Материал.Код_материала = Поставка_материал.Код_материала
GROUP BY Материал.Название;


--9. Найти все обработки определенного этапа для определенной заготовки.
SELECT *
FROM Обработка
WHERE Код_этапа = 1 AND Код_заготовки = 1;


--10. Показать список всех поставщиков и количество поставок каждого из них за последние 12 месяцев.
SELECT Поставщик.Название, COUNT(Поставка_поставщик.Код_поставки) AS Количество_поставок
FROM Поставщик
JOIN Поставка_поставщик ON Поставщик.Код_поставщика = Поставка_поставщик.Код_поставщика
WHERE Поставка_поставщик.Дата_поставки >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY Поставщик.Название;


-----------------------ПРЕДСТАВЛЕНИЯ
--1 Представление для отображения списка всех заказов с указанием клиента, даты заказа, статуса заказа и суммы заказа.
CREATE VIEW Все_заказы AS
SELECT Заказ.Код_заказа, Клиент.Фамилия, Клиент.Имя, Заказ.Дата_заказа, Статус.Название AS Статус_заказа, Заказ.Сумма_заказа
FROM Заказ
JOIN Клиент ON Заказ.Код_клиента = Клиент.Код_клиента
JOIN Статус ON Заказ.Код_статуса = Статус.Код_статуса;
select * from Все_заказы;



--2. Представление для отображения списка всех поставок материалов с указанием поставщика, даты поставки.
CREATE VIEW Поставки_материалов AS
SELECT Поставка_поставщик.Дата_поставки, Поставщик.Название
FROM Поставка_поставщик
JOIN Поставщик ON Поставка_поставщик.Код_поставщика = Поставщик.Код_поставщика
JOIN Поставка_материал ON Поставка_поставщик.Код_поставки = Поставка_материал.Код_поставки
JOIN Материал ON Поставка_материал.Код_материала = Материал.Код_материала
GROUP BY Поставка_поставщик.Дата_поставки, Поставщик.Название;
select * from Поставки_материалов;



--3. Представление для отображения списка всех клиентов и общей суммы всех их заказов за последний год.

CREATE VIEW Общая_сумма_заказов AS
SELECT Клиент.Фамилия, Клиент.Имя, SUM(Заказ.Сумма_заказа) AS Общая_сумма
FROM Клиент
JOIN Заказ ON Клиент.Код_клиента = Заказ.Код_клиента
WHERE Заказ.Дата_заказа >= CURRENT_DATE - INTERVAL '1 year'
GROUP BY Клиент.Фамилия, Клиент.Имя;
select * from Общая_сумма_заказов;



---------------------------Процедуры



--1. Процедура добавления нового поставщика:

CREATE OR REPLACE PROCEDURE add_supplier(
    IN last_name VARCHAR(255),
    IN first_name VARCHAR(255),
    IN middle_name VARCHAR(255),
    IN address VARCHAR(255),
    IN phone_number VARCHAR(255),
    IN name VARCHAR(255)
)
LANGUAGE plpgsql
AS
$$
BEGIN
    INSERT INTO Поставщик (Фамилия, Имя, Отчество, Адрес, Номер_телефона, Название)
    VALUES (last_name, first_name, middle_name, address, phone_number, name);
END;
$$;



--Пример теста:
CALL add_supplier('Иванов', 'Иван', 'Иванович', 'ул. Ленина 1', '+7 123 456-78-90', 'ООО Рога и копыта');
SELECT * FROM Поставщик WHERE Фамилия = 'Иванов';



--2. Процедура добавления новой поставки:

CREATE OR REPLACE PROCEDURE add_delivery(
    IN supplier_id INT,
    IN material_id INT,
    IN quantity INT,
    IN delivery_date DATE,
    IN delivery_amount INT
)
LANGUAGE plpgsql
AS 
$$
    BEGIN
        INSERT INTO Поставка_поставщик (Код_поставщика, Сумма_поставки, Дата_поставки) 
        VALUES (supplier_id, delivery_amount, delivery_date);
        
        INSERT INTO Поставка_материал (Код_поставки, Код_материала, Количество_в_листах) 
        VALUES (currval(pg_get_serial_sequence('Поставка_поставщик', 'Код_поставки')), material_id, quantity);
        
        UPDATE Материал SET Количество_в_листах = Количество_в_листах + quantity WHERE Код_материала = material_id;
    END;
$$;

--Пример теста:
INSERT INTO Поставщик (Фамилия, Имя, Отчество, Адрес, Номер_телефона, Название) 
VALUES ('Петров', 'Петр', 'Петрович', 'ул. Пушкина 2', '+7 123 456-78-91', 'ООО Копье и щит');
INSERT INTO Материал (Единица_измерения, Количество_в_листах, Название) VALUES ('лист', 0, 'Сталь');
CALL add_delivery(1, 1, 100, CURRENT_DATE, 50000);
SELECT * FROM Поставка_материал WHERE Код_материала = 1;

-- 3. Процедура добавления новой обработки:

CREATE OR REPLACE PROCEDURE add_processing(
    IN blank_id INT,
    IN stage_name CHAR(255),
    IN quantity INT,
    IN processing_date DATE
)
LANGUAGE plpgsql
AS 
$$
    DECLARE
        stage_id INT;
    BEGIN
        SELECT Код_этапа INTO stage_id FROM Этапы_обработки WHERE Название = stage_name;
        INSERT INTO Обработка (Код_заготовки, Код_этапа, Количетсво, Дата) 
        VALUES (blank_id, stage_id, quantity, processing_date);
        
        UPDATE Заготовка SET Количество = Количество - quantity WHERE Код_заготовки = blank_id;
    END;
$$;

--Пример теста:
INSERT INTO Этапы_обработки (Название) VALUES ('Резка');
INSERT INTO Заказ (Код_клиента, Код_статуса, Дата_заказа) VALUES (1, 1, CURRENT_DATE);
INSERT INTO Изделие (Стоимость, Шаблон) VALUES (100000, 'Шаблон 1');
INSERT INTO Связь_заказ_изделие (Код_заказа, Код_изделия, Количество_в_листах) VALUES (1, 1, 10);
INSERT INTO Заготовка (Количество, Код_связь_заказ_изделие, Размер_см_кв) VALUES (100, 1, 50);
CALL add_processing(1, 'Резка', 20, CURRENT_DATE);
SELECT * FROM Обработка WHERE Код_заготовки = 1;


--4. Процедура для добавления нового заказа
CREATE OR REPLACE PROCEDURE Добавить_заказ(
П_код_клиента int,
П_код_статуса int,
П_дата_заказа date,
П_сумма_заказа int
)
LANGUAGE plpgsql
AS
$$
BEGIN
INSERT INTO Заказ (Код_клиента, Код_статуса, Дата_заказа, Сумма_заказа)
VALUES (П_код_клиента, П_код_статуса, П_дата_заказа, П_сумма_заказа);
END;
$$;

-- Тест для процедуры Добавить_заказ
CALL Добавить_заказ(1, 1, CURRENT_DATE, 1000);
SELECT * FROM Заказ;


--5. Процедура для обновления статуса заказа
CREATE OR REPLACE PROCEDURE Обновить_статус_заказа(
П_код_заказа int,
П_код_статуса int
)
LANGUAGE plpgsql
AS
$$
BEGIN
UPDATE Заказ
SET Код_статуса = П_код_статуса
WHERE Код_заказа = П_код_заказа;
END;
$$;


-- Тест для процедуры Обновить_статус_заказа
CALL Обновить_статус_заказа(1, 5);
SELECT * FROM Заказ;




--1. Функция для подсчета общей стоимости всех заказов клиента
CREATE OR REPLACE FUNCTION Получить_общую_стоимость_заказов(
П_код_клиента int
)
RETURNS int
LANGUAGE plpgsql
AS
$$
DECLARE
    П_сумма int;
BEGIN
    SELECT SUM(Сумма_заказа) INTO П_сумма
    FROM Заказ
    WHERE Код_клиента = П_код_клиента;

    RETURN П_сумма;
END;
$$;

-- Тест для функции Получить_общую_стоимость_заказов
SELECT Получить_общую_стоимость_заказов(1);


--2. Функция для проверки наличия материала в заготовке
CREATE OR REPLACE FUNCTION Проверить_наличие_материала(
П_код_материала int,
П_код_заготовки int
)
RETURNS boolean
LANGUAGE plpgsql
AS
$$
DECLARE
    П_наличие boolean;
BEGIN
    SELECT COUNT(*)
    INTO П_наличие
    FROM Связь_материал_заготовка
    WHERE Код_материала = П_код_материала AND Код_заготовки = П_код_заготовки;

    RETURN П_наличие;
END;
$$;

-- Тест для функции Проверить_наличие_материала
SELECT Проверить_наличие_материала(1, 1);


--3. Функция для получения списка всех клиентов
CREATE OR REPLACE FUNCTION Получить_список_клиентов()
RETURNS TABLE (Код_клиента int,Фамилия char(255), Имя char(255), Отчество char(255), Адрес char(255), Номер_телефона char(255))
LANGUAGE plpgsql
AS
$$
BEGIN
RETURN QUERY SELECT Клиент.Код_клиента, Клиент.Фамилия, Клиент.Имя, Клиент.Отчество, Клиент.Адрес,Клиент.Номер_телефона
FROM Клиент;
END;
$$;
-- Тест для функции Получить_список_клиентов
SELECT * FROM Получить_список_клиентов();



-----------------------------------РОЛИ

-- revoke all privileges on
-- database postgres from Клиент;
-- revoke all privileges on all tables 
-- in schema public from Клиент;

-- drop role if exists Клиент;
-- Create ROLE Клиент;
-- GRANT USAGE ON SCHEMA public TO Клиент;
-- GRANT SELECT ON public.Заказ,public.Изделие TO Клиент;


-- revoke all privileges on
-- database postgres from Маркетплейс;
-- revoke all privileges on all tables 
-- in schema public from Маркетплейс;
-- drop role if exists Маркетплейс;
-- Create ROLE Маркетплейс;
-- GRANT SELECT,INSERT,UPDATE ON Заказ TO Маркетплейс;
-- GRANT SELECT,INSERT,UPDATE ON Изделие TO Маркетплейс;


-- revoke all privileges on
-- database postgres from Администратор;
-- revoke all privileges on all tables 
-- in schema public from Администратор;
-- drop role if exists Администратор;
-- Create ROLE Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Заказ TO Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Изделие TO Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Материал TO Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Обработка TO Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Поставка_материал TO Администратор;
-- GRANT SELECT,INSERT,UPDATE ON Поставка_поставщик TO Администратор;


-- revoke all privileges on
-- database postgres from Директор;
-- revoke all privileges on all tables 
-- in schema public from Директор;
-- drop role if exists Директор;
-- Create ROLE Директор;
-- GRANT ALL PRIVILEGES ON DATABASE postgres TO Директор;
--КАК СОЗДАТЬ РЕЗЕРВНУЮ КОПИЮ
-- drop role if exists reserve;
-- CREATE USER reserve WITH PASSWORD '123456';
--1.ЗАЙТИ В КОНСОЛЬ
--2.cd C:\Program Files\PostgreSQL\15(версия sql)\bin
--Создание резервной копии БД
--pg_dump -U reserve postgres > my_backup.dump

--Восстановление из резервной копии
--pg_restore -U reserve -d postgres my_backup.sql


