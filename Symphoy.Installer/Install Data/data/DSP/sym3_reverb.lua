--#API_VERSION=1
--#ON_DSP=false
--#AFTER_DSP=true
--#TITLE=Echo Lua.ver
--#AUTHOR=AinL
--#DESCRIBE=울림 효과입니다
--#RELEASE_DATE=2016.07.27
--#VERSION=0.001

reverb_len = 1
reverb_decoy = 0.28

a = {}
a_count = 0
a_max = 10000
a_index = 0
function Init (channel, sampleRate)
	collectgarbage()
	--a = {}
	a_count = 0
	a_max = tonumber( sampleRate ) * reverb_len * 2.0
	a_index = 0
end

function Apply (channel, sample, index, count)
	return sample
end

function ArrayApply (buffer, offset, count)	
	for i=0, count-1 do
		-- TODO Same as Apply
		if a_count < a_max then
			a_count = a_count + 1
			a[a_count] = buffer[i]
		else
			a_index = a_index + 1
			if a_index == a_max then
				a_index = 1
			end
			--a[a_index] = buffer[i]
		end
		
		--get sample
		getSample = -100
		if a_count == a_max then
			if a_index == a_max - 1 then
				getSample = a[1]
			else
				getSample = a[a_index + 1]
			end
		end
		
		if getSample ~= -100 then
			buffer[i] = buffer[i] * (1-reverb_decoy) + getSample * reverb_decoy
			a[a_index] = buffer[i]
		end
		getSample = nil
	end
end