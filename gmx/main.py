import typer
from gmx.cmd.workflow import workflow_app
from gmx.cmd.project import project_app

app = typer.Typer()
app.add_typer(workflow_app, name="wf")
app.add_typer(project_app, name="proj")

if __name__ == "__main__":
    app()