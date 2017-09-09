--#API_VERSION=1
--#ON_DSP=false
--#AFTER_DSP=true
--#TITLE=Paser Lua.ver
--#AUTHOR=AinL
--#DESCRIBE=페이서 입니다
--#RELEASE_DATE=2016.07.27
--#VERSION=0.001

paser_len = 5
paser_fac = 0.3

function Init (channel, sampleRate)
	lent = paser_len * sampleRate
end

lent = 10000
function Apply (channel, sample, index, count)
	return sample
end

ind = 0
function ArrayApply (buffer, offset, count)	
	for i=0, count-1 do
		ind = ind + 1
		if ind == lent then
			ind = 0
		end
		buffer[i] = buffer[i] * (1-paser_fac) + buffer[i] * paser_fac * math.sin( ind / lent * 3.14 * 2)
	end
end