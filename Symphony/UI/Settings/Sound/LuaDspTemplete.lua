--#API_VERSION=1
--#ON_DSP=true
--#AFTER_DSP=false
--#TITLE=제목
--#AUTHOR=작성자
--#DESCRIBE=설명
--#RELEASE_DATE=날자
--#VERSION=버젼

function Init (channel, sampleRate)
	
end

function Apply (channel, sample, index, count)
	return sample
end

function ArrayApply (buffer, offset, count)
	return buffer
end