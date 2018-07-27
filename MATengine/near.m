function [Idx] = near(X,val,tolerance)

% NEAR.M:
% [Idx]=near(X,val,tolerance);
%
% The function accepts a vector or matrix 'X', a desired value 'val' and a
% tolerance level 'tolerance'. It is used throughout the code associated
% with the dissertation.
%
% When X is a vector the function returns the index of the element nearest
% the desired value 'val'. If no element is within the tolerance level Idx
% is returned empty.
%
% When X is an nxm matrix the function returns the 1xm vector Idx. Each
% element in Idx is the row number of the element in the corresponding
% column of X with value nearest the desired value 'val'. If no element is
% found within the tolerance level a zero is returned.

[n,m] = size(X);

if exist('tolerance') == 1
    tol = tolerance;
else
    tol = 10^10;
end

Err = abs(X - val*ones(n,m));

if n+m == max(n,m)+1     %X is Vector
    if min(Err) < tol
        loc = find(Err == min(Err));
        if length(loc) == 1
            Idx = loc;
        else
            %'Error - two values equally near value'
            Idx = [];
        end
    else
        %'No values within tolerance'
        Idx = [];
    end
else    %X is Matrix
    Idx = zeros(1,m);
    for c = 1:m
        if min(Err(:,c)) < tol
            loc = find(Err(:,c) == min(Err(:,c)));
            if length(loc) == 1
                Idx(c) = loc;
            else
                Idx(c) = loc(1);
            end
        else
            %'No value within tolerance'
        end
    end
end
    
% if min(min(Idx)) == 0
%     'Idx contains zeros - two values equally near value OR no data within tolerance of value'
% end