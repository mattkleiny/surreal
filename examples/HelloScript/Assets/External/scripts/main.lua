local timer = 0

function tick(game, delta_time)
  game:Clear(0.2, 0.2, 0.2, 0.8)

  print("T minus " .. 10 - timer .. " seconds until shutdown")

  timer = timer + delta_time
  if timer >= 10 then
    game:Exit()
  end

  return timer, 10 - timer
end
