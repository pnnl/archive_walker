function [Projects, Tasks] = ListProjectsTasks(ProjectPath)

ProjectFolders = dir(ProjectPath);
ProjectFolders = {ProjectFolders.name};
ProjectFolders = ProjectFolders(contains(ProjectFolders,'Project_'));

Projects = cell(1,length(ProjectFolders));
Tasks = cell(1,length(ProjectFolders));
for Pidx = 1:length(ProjectFolders)
    Projects{Pidx} = ProjectFolders{Pidx}(9:end);
    
    TaskFolders = dir(fullfile(ProjectPath,ProjectFolders{Pidx}));
    TaskFolders = {TaskFolders.name};
    TaskFolders = TaskFolders(contains(TaskFolders,'Task_'));
    
    Tasks{Pidx} = cell(1,length(TaskFolders));
    for Tidx = 1:length(TaskFolders)
        Tasks{Pidx}{Tidx} = TaskFolders{Tidx}(6:end);
    end
end