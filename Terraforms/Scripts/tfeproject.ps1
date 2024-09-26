az pipelines run --organization 'https://dev.azure.com/cbre/' --project 'Cloud' --id 16752 `
  --variables `
    project_name='WintelDevOps' `
    team_names='WintelDevops' `
    request_number="null" `
    sys_id="null" `
  --parameters `
    test_pipeline=true