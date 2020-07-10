module Cookbook.Client.View

open Cookbook.Client.State

open Fable.React
open Feliz
open Feliz.MaterialUI


let private useToolbarStyles = Styles.makeStyles(fun styles theme ->
  {|
    appBarTitle = styles.create [
      style.flexGrow 1
    ]
  |}
)

let private useRootViewStyles : unit -> _ = Styles.makeStyles(fun styles theme ->
  let drawerWidth = 240
  {|
    root = styles.create (fun model -> [
      style.display.flex
      style.userSelect.none
    ])
    appBar = styles.create [
      style.zIndex (theme.zIndex.drawer + 1)
    ]
    drawer = styles.create [
      style.width (length.px drawerWidth)
      style.flexShrink 0
    ]
    drawerPaper = styles.create [
      style.width (length.px drawerWidth)
      // Example of breakpoint media queries
      style.inner theme.breakpoints.downXs [
        style.backgroundColor.red
      ]
    ]
    content = styles.create [
      style.flexGrow 1
      style.padding (theme.spacing 3)
    ]
    toolbar = styles.create [
      yield! theme.mixins.toolbar
    ]
  |}
)

let Toolbar = React.functionComponent(fun (model, dispatch) ->
  let c = useToolbarStyles ()
  Mui.toolbar [
    Mui.typography [
      typography.variant.h6
      typography.color.inherit'
      typography.children "Cookbook"
      typography.classes.root c.appBarTitle
    ]
    Mui.button [
        prop.text "Login"
        prop.onClick (fun _ -> PageChanged Page.Login |> dispatch)
        button.color.inherit'
    ]
  ]
)

let private pageListItem (page:string) =
  Mui.listItem [
    prop.key page
    listItem.button true
    listItem.children [
      Mui.listItemText  page
    ]
  ]


let main = React.functionComponent(fun (model, dispatch) ->
    let appStyles = useRootViewStyles ()
    Fable.Core.JS.console.log(appStyles.root)

    let lightTheme = Styles.createMuiTheme([
        theme.palette.type'.light
        theme.palette.primary Colors.indigo
        theme.palette.secondary Colors.pink
      ])

    Mui.themeProvider [
        themeProvider.theme lightTheme
        themeProvider.children [
            Html.div [
                prop.className appStyles.root
                prop.children [
                    Mui.cssBaseline []
                    Mui.appBar [
                        appBar.classes.root appStyles.appBar
                        appBar.position.fixed'
                        appBar.children [
                            Toolbar(model, dispatch)
                        ]
                    ]
                    Mui.drawer [
                        drawer.variant.permanent
                        drawer.classes.root appStyles.drawer
                        drawer.classes.paper appStyles.drawerPaper
                        drawer.children [
                            Html.div [ prop.className appStyles.toolbar ]
                            Mui.list [
                                list.component' "nav"
                                list.children (["test"] |> List.map pageListItem |> ofList)
                            ]
                        ]
                    ]




                ]
            ]
        ]
    ]



)

let render (model:State.Model) (dispatch: Msg -> unit) =
  main (model, dispatch)