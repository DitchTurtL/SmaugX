
DO $$ 
BEGIN

IF NOT EXISTS (SELECT 1 FROM users WHERE id=1) THEN
    insert into users (name, email, password, permissions)
    values ('Test', 'test@test.com', 'Azwu722q7w4Hh1MplBgMfbgYlq2/U/WVekGGUFLj9s0E2WC1rc/73eTFvYoTIQlo', 2369);

    insert into characters (user_id, name, current_room_id, permissions)
    values (1, 'Tester', 1, 2369);

    insert into rooms (name, short_description, long_description)
    values ('The Void', 'You are in the void.', 'You are in the void.');
END IF;

END $$;