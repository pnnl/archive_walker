function Mpath = GetRetConPath(Mtrack)

% When a mode was not identified, it is returned as NaN in Mtrack. Starting
% at the end of Mtrack, work backwards and replace NaN values with the
% upcoming mode estimate. Distances between adjacent NaNs are then zero,
% but the distance from the pole estimate preceeding the NaNs to after the
% NaNs is retained.
MtrackOrig = Mtrack;
LvHold = Mtrack{end};
for LvIdx = length(Mtrack)-1:-1:2
    if length(Mtrack{LvIdx}) == 1
        % If length of 1, it should be NaN. Double check
        if isnan(Mtrack{LvIdx})
            Mtrack{LvIdx} = LvHold;
            MtrackOrig{LvIdx} = NaN(size(LvHold));
        end
    end
    LvHold = Mtrack{LvIdx};
end

PtNum = Mtrack;
PtNumer = 1;
Mpull = [];
for k = 1:length(Mtrack)
    for kk = 1:length(Mtrack{k})
        PtNum{k}(kk) = PtNumer;
        PtNumer = PtNumer + 1;
        Mpull = [Mpull MtrackOrig{k}(kk)];
    end
end

s = [];
t = [];
w = [];
% For each of the levels before the last one
for LvlIdx = 1:length(Mtrack)-1
    % For each of the points in the level
    for PtIdx = 1:length(Mtrack{LvlIdx})
        % For each point in the next level
        for NextLvlPtIdx = 1:length(Mtrack{LvlIdx+1})
            % From point
            s = [s PtNum{LvlIdx}(PtIdx)];
            % To point
            t = [t PtNum{LvlIdx+1}(NextLvlPtIdx)];
            % Distance between the points
            %**************************************************************
            % Note that this weight could be adjusted to reflect the amount
            % of pseudo-energy that the mode estimate had
            %**************************************************************
            w = [w abs(Mtrack{LvlIdx}(PtIdx) - Mtrack{LvlIdx+1}(NextLvlPtIdx))];
        end
    end
end

G = graph(s,t,w);
P = shortestpath(G,1,t(end));
Mpath = Mpull(P(2:end-1));