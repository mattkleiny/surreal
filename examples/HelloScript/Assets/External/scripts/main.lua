local timer = 0 -- remember local state

function tick(game, delta_time)
  -- clear the main display
  game:Clear(0.2, 0.2, 0.2, 0.8)

  -- tick down a timer, exit when done
  timer = timer + delta_time
  print("T minus " .. 10 - timer .. " seconds until shutdown")

  if timer >= 10 then
    game:Exit()
  end

  -- draw a cross pattern
  game:DrawLine(Vector2(-1, -1), Vector2(1, 1))
  game:DrawLine(Vector2(-1, 1), Vector2(1, -1))

  -- return multiple values
  return timer, 10 - timer
end
