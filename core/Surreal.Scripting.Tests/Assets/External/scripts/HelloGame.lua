function tick(delta_time)
  print("Time elapsed " .. delta_time)

  Game:RaiseEvent("Hello, world!")
end
