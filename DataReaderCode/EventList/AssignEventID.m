function ID = AssignEventID()

persistent IDtrack

if isempty(IDtrack)
    IDtrack = 1;
else
    IDtrack = IDtrack + 1;
end

if IDtrack >= 10000
    IDtrack = 1;
end

ID = num2str(now()+IDtrack,'%.11f');