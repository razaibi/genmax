import typer
from gmx.logic.project import ProjectLogic

project_app = typer.Typer()

@project_app.command(name="add")
def add_project(name:str):
    """
    Add a project.

    Args:
        name (str): The name of the project to create.
    """
    typer.echo(f"Adding project {name}.")
    pl = ProjectLogic()
    pl.add_project(name)
    typer.echo("Status : Done.")
    typer.echo("")
    typer.echo("Run a sample project using the below command:")
    typer.echo(f"gmx wf run {name} sample")
    