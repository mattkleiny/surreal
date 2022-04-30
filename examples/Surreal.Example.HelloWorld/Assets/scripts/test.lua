﻿require("Assets/scripts/utils.lua")

time_elapsed = 0 -- local timer

-- core update loop
function update(delta_time)
  time_elapsed = time_elapsed + delta_time

  if time_elapsed > 1 then
    say_hello("it has been " .. delta_time .. " seconds since the last frame")
    time_elapsed = 0
  end
end
