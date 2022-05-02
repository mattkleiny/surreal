total_time = 0

function update(seconds)
  total_time = total_time + seconds

  if (total_time > 1) then
    print("Hello, World!")
    total_time = 0
  end
end
