--#API_VERSION=1
--#ON_DSP=false
--#AFTER_DSP=true
--#TITLE=Stereo Enhancer
--#AUTHOR=AinL
--#DESCRIBE=스테레오를 강화시켜줍니다
--#RELEASE_DATE=2016.07.27
--#VERSION=0.0.1

function Init (channel, sampleRate)
	
end

function Apply (channel, sample, index, count)
	return sample
end

chInd = 0
presample = 0

fact = 1.5
function ArrayApply (buffer, offset, count)
	for i=0, count-1 do
		chInd = chInd + 1
		if chInd == 2 then
			chInd = 0
			mono = buffer[i-1] * 0.5 + buffer[i] * 0.5
			delta =  (buffer[i] - mono) * fact
			buffer[i] = mono + delta
			buffer[i-1] = mono - delta
		end
	end
end