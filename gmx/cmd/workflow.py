from typing import List
import typer
from gmx.logic.workflow import WorkFlowLogic

workflow_app = typer.Typer()

@workflow_app.command(name="run")
def run_workflow(project:str,  workflows: List[str]):
    """
    Run a workflow.

    Args:
        name (str): The name of the project with the workflows.
        workflows (list): List of workflows to run.
    """
    typer.echo("Adding a task")
    WorkFlowLogic.run_workflows(project, workflows)