--#API_VERSION=1
-- Data Field for Wrapper
--#ON_DSP=true
--#AFTER_DSP=false
-- Meta data
--#TITLE=Symphony Demo Lua DSP Script
--#AUTHOR=AinL
--#DESCRIBE=루아스크립트 불러오기
--#RELEASE_DATE=2016.07.27
--#VERSION=0.0.1
-- Symphony Demo Lua DSP Script
-- Made by AinL

q_count = 0
q_max = 10000
strength = 0.7

pl = true

function Init(channel, sampleRate)
end

function Apply (channel, sample, index, count)
	if q_count > q_max then
		pl = false
	elseif q_count < 0 then
		pl = true
	end

	if pl then
		q_count = q_count + 1
	else
		q_count = q_count - 1
	end

	return sample * (( q_count / q_max )*strength + (1-strength))
end

function ArrayApply(buffer, offset, count)
	return buffer
end